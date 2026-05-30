using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using LibVLCSharp.Shared;

namespace EasyVideoScreensaver
{
    public partial class App : Application
    {
        private HwndSource previewHwndSource;
        private VideoWindow mainWindow;
        private readonly List<MediaPlayer> mediaPlayers = new List<MediaPlayer>();

        public MySettings settings;
        public string settingsFilename;

        void ApplicationStartup(object sender, StartupEventArgs e)
        {
            settingsFilename = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\VideoScreensaver.xml";

            settings = MySettings.Load(settingsFilename);

            string[] args = e.Args;

            if (e.Args.Length > 0 && e.Args[0].Contains(":"))
                args = e.Args[0].Split(':');

            if (args.Length == 0)
            {
                ShowSettings();
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "/s":
                        ShowScreensaver();
                        return;
                    case "/p":
                        if (e.Args.Length > 1)
                        {
                            int handle = Convert.ToInt32(e.Args[1]);
                            IntPtr hWnd = new IntPtr(handle);
                            ShowPreview(hWnd);
                        }
                        return;
                    case "/c":
                        ShowSettings();
                        return;
                    default:
                        MessageBox.Show("Invalid command line parameter: " + args[0], "Video Screensaver", MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                        return;
                }
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (var player in mediaPlayers)
            {
                player.Stop();
                player.Dispose();
            }
            mediaPlayers.Clear();
            VlcPlayback.Dispose();
            base.OnExit(e);
        }

        private void ShowScreensaver()
        {
            if (!HasVideoFile())
                return;

            foreach (Monitor m in Monitor.AllMonitors)
            {
                MediaPlayer player = CreatePlayer();
                mediaPlayers.Add(player);

                VideoWindow window = new VideoWindow(player);
                window.Top = m.Bounds.Top / (m.DpiY / 96);
                window.Left = m.Bounds.Left / (m.DpiX / 96);
                window.Height = m.Bounds.Height / (m.DpiY / 96);
                window.Width = m.Bounds.Width / (m.DpiX / 96);
                window.Show();
            }
        }

        private void ShowPreview(IntPtr pPreviewHnd)
        {
            if (!HasVideoFile())
                return;

            MediaPlayer player = CreatePlayer();
            mediaPlayers.Add(player);

            mainWindow = new VideoWindow(player);

            NativeMethods.RECT lpRect = new NativeMethods.RECT();
            NativeMethods.GetClientRect(pPreviewHnd, out lpRect);

            HwndSourceParameters sourceParams = new HwndSourceParameters("sourceParams");
            sourceParams.PositionX = 0;
            sourceParams.PositionY = 0;
            sourceParams.Height = lpRect.Height;
            sourceParams.Width = lpRect.Width;
            sourceParams.ParentWindow = pPreviewHnd;
            sourceParams.WindowStyle = (int)(NativeMethods.WindowStyles.WS_VISIBLE | NativeMethods.WindowStyles.WS_CHILD | NativeMethods.WindowStyles.WS_CLIPCHILDREN);

            previewHwndSource = new HwndSource(sourceParams);
            previewHwndSource.Disposed += previewHwndSource_Disposed;
            previewHwndSource.RootVisual = mainWindow.Display;
            mainWindow.EnsurePlaybackStarted();
        }

        private void ShowSettings()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        void previewHwndSource_Disposed(object sender, EventArgs e)
        {
            mainWindow.Close();
            Application.Current.Shutdown();
        }

        private bool HasVideoFile()
        {
            return !string.IsNullOrEmpty(settings.VideoFilename) && System.IO.File.Exists(settings.VideoFilename);
        }

        private MediaPlayer CreatePlayer()
        {
            return VlcPlayback.CreatePlayer(settings);
        }
    }
}
