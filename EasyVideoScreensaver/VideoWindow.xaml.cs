using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using LibVLCSharp.Shared;

namespace EasyVideoScreensaver
{
    public partial class VideoWindow : Window
    {
        private readonly MySettings settings = ((App)Application.Current).settings;
        private readonly string settingsFilename = ((App)Application.Current).settingsFilename;
        private readonly MediaPlayer mediaPlayer;
        private bool playbackStarted;
        private DispatcherTimer inputTimer;
        private NativeMethods.POINT initialCursorPos;
        private bool wasMouseDown;
        private const int MouseMoveThreshold = 5;

        public VideoWindow(MediaPlayer player)
        {
            InitializeComponent();
            mediaPlayer = player;
            VideoView.MediaPlayer = mediaPlayer;
            Loaded += VideoWindow_Loaded;
        }

        private void VideoWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TryStartPlayback();
            StartInputMonitoring();
        }

        private void TryStartPlayback()
        {
            if (playbackStarted)
                return;

            if (!VideoView.IsLoaded)
            {
                Dispatcher.BeginInvoke(new Action(TryStartPlayback), DispatcherPriority.Loaded);
                return;
            }

            playbackStarted = true;

            if (!mediaPlayer.IsPlaying)
                mediaPlayer.Play();

            UpdateStretch();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateStretch();
        }

        private void UpdateStretch()
        {
            if (mediaPlayer == null)
                return;

            VlcPlayback.ApplyStretchToWindow(
                mediaPlayer,
                settings.StretchMode,
                (int)ActualWidth,
                (int)ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            CloseScreensaver();
        }

        private void StartInputMonitoring()
        {
            NativeMethods.GetCursorPos(out initialCursorPos);
            wasMouseDown = IsAnyMouseButtonDown();

            inputTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
            inputTimer.Tick += InputTimer_Tick;
            inputTimer.Start();
        }

        private void InputTimer_Tick(object sender, EventArgs e)
        {
            NativeMethods.GetCursorPos(out NativeMethods.POINT currentPos);
            int dx = currentPos.X - initialCursorPos.X;
            int dy = currentPos.Y - initialCursorPos.Y;
            if (dx * dx + dy * dy > MouseMoveThreshold * MouseMoveThreshold)
            {
                CloseScreensaver();
                return;
            }

            bool isMouseDown = IsAnyMouseButtonDown();
            if (!wasMouseDown && isMouseDown)
                CloseScreensaver();

            wasMouseDown = isMouseDown;
        }

        private static bool IsAnyMouseButtonDown()
        {
            return (NativeMethods.GetAsyncKeyState(NativeMethods.VK_LBUTTON) & 0x8000) != 0
                || (NativeMethods.GetAsyncKeyState(NativeMethods.VK_RBUTTON) & 0x8000) != 0
                || (NativeMethods.GetAsyncKeyState(NativeMethods.VK_MBUTTON) & 0x8000) != 0;
        }

        private void CloseScreensaver()
        {
            inputTimer?.Stop();

            if (settings.Resume)
            {
                settings.ResumePosition = VlcPlayback.GetPositionSeconds(mediaPlayer);
                settings.Save(settingsFilename);
            }

            Application.Current.Shutdown();
        }
    }
}
