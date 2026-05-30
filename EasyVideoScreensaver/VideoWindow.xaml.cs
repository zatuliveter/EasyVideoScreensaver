using System.Windows;
using System.Windows.Input;
using LibVLCSharp.Shared;

namespace EasyVideoScreensaver
{
    public partial class VideoWindow : Window
    {
        private readonly MySettings settings = ((App)Application.Current).settings;
        private readonly string settingsFilename = ((App)Application.Current).settingsFilename;
        private readonly MediaPlayer mediaPlayer;

        public VideoWindow(MediaPlayer player)
        {
            InitializeComponent();
            mediaPlayer = player;
            VideoView.MediaPlayer = player;
            ApplyVideoViewLayout();
            Loaded += VideoWindow_Loaded;
        }

        public UIElement Display => VideoView;

        private void ApplyVideoViewLayout()
        {
            if (settings.StretchMode == "Center")
            {
                VideoView.HorizontalAlignment = HorizontalAlignment.Center;
                VideoView.VerticalAlignment = VerticalAlignment.Center;
            }
            else
            {
                VideoView.HorizontalAlignment = HorizontalAlignment.Stretch;
                VideoView.VerticalAlignment = VerticalAlignment.Stretch;
            }
        }

        private void VideoWindow_Loaded(object sender, RoutedEventArgs e)
        {
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

        private void Window_MouseDown(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            CloseScreensaver();
        }

        private void CloseScreensaver()
        {
            if (settings.Resume)
            {
                settings.ResumePosition = VlcPlayback.GetPositionSeconds(mediaPlayer);
                settings.Save(settingsFilename);
            }

            Application.Current.Shutdown();
        }
    }
}
