using System;
using System.Collections.Generic;
using System.Windows;
using LibVLCSharp.Shared;

namespace EasyVideoScreensaver
{
    public partial class App : Application
    {
        private readonly List<MediaPlayer> mediaPlayers = new List<MediaPlayer>();

        public MySettings settings;
        public string settingsFilename;

        void ApplicationStartup(object sender, StartupEventArgs e)
        {
            string[] args = e.Args;

            if (e.Args.Length > 0 && e.Args[0].Contains(":"))
                args = e.Args[0].Split(':');

            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "/p":
                        Shutdown();
                        return;
                }
            }

            settingsFilename = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\VideoScreensaver.xml";

            settings = MySettings.Load(settingsFilename);

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
                    case "/c":
                        ShowSettings();
                        return;
                    default:
                        MessageBox.Show("Invalid command line parameter: " + args[0], "Video Screensaver", MessageBoxButton.OK, MessageBoxImage.Error);
                        Shutdown();
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

        private void ShowSettings()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
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
