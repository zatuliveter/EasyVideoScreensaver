using System;
using System.Globalization;
using LibVLCSharp.Shared;

namespace EasyVideoScreensaver
{
    public static class VlcPlayback
    {
        private static LibVLC libVlc;
        private static bool initialized;

        public static void EnsureInitialized()
        {
            if (initialized)
                return;

            Core.Initialize();
            // direct3d11 has proper HDR tone mapping (same as updated VLC). Safe now that playback
            // starts only after VideoView is loaded — otherwise LibVLC opens a separate D3D11 window.
            libVlc = new LibVLC(
                "--vout=direct3d11",
                "--tone-mapping=3",
                "--tone-mapping-desat=0.2",
                "--no-video-title-show");
            initialized = true;
        }

        public static MediaPlayer CreatePlayer(MySettings settings)
        {
            EnsureInitialized();

            var media = CreateMedia(settings);
            var player = new MediaPlayer(media);
            var videoPath = settings.VideoFilename;

            ApplyStretch(player, settings.StretchMode);
            player.Volume = settings.Mute ? 0 : (int)(settings.Volume * 100);
            player.Mute = settings.Mute;

            player.EndReached += (sender, e) =>
            {
                System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    player.Stop();
                    var loopMedia = new Media(libVlc, videoPath, FromType.FromPath);
                    player.Media = loopMedia;
                    loopMedia.Dispose();
                    player.Play();
                }));
            };

            return player;
        }

        private static Media CreateMedia(MySettings settings)
        {
            var media = new Media(libVlc, settings.VideoFilename, FromType.FromPath);
            if (settings.Resume && settings.ResumePosition > 0)
            {
                media.AddOption(string.Format(
                    CultureInfo.InvariantCulture,
                    ":start-time={0}",
                    settings.ResumePosition));
            }

            return media;
        }

        public static void ApplyStretch(MediaPlayer player, string stretchMode)
        {
            switch (stretchMode)
            {
                case "Fill":
                    player.AspectRatio = null;
                    player.Scale = 0;
                    player.CropGeometry = null;
                    break;
                case "Center":
                    player.AspectRatio = null;
                    player.Scale = 1;
                    player.CropGeometry = null;
                    break;
                default:
                    player.AspectRatio = null;
                    player.Scale = 0;
                    player.CropGeometry = null;
                    break;
            }
        }

        public static void ApplyStretchToWindow(MediaPlayer player, string stretchMode, int windowWidth, int windowHeight)
        {
            if (windowWidth <= 0 || windowHeight <= 0)
                return;

            switch (stretchMode)
            {
                case "Fill":
                    player.AspectRatio = $"{windowWidth}:{windowHeight}";
                    player.Scale = 0;
                    break;
                case "Center":
                    player.AspectRatio = null;
                    player.Scale = 1;
                    break;
                default:
                    player.AspectRatio = null;
                    player.Scale = 0;
                    break;
            }
        }

        public static double GetPositionSeconds(MediaPlayer player)
        {
            if (player == null || player.Time < 0)
                return 0;
            return player.Time / 1000.0;
        }

        public static void Dispose()
        {
            if (libVlc != null)
            {
                libVlc.Dispose();
                libVlc = null;
            }
            initialized = false;
        }
    }
}
