# Easy Video Screensaver

*Play video as screensaver*

## Overview

Easy Video Screensaver plays a video as a screensaver.

## Supported File Types

Easy Video Screensaver uses [LibVLC](https://www.videolan.org/vlc/) (the same engine as VLC media player) to decode and display video. Most formats VLC can play are supported, including HDR10 content with automatic tone mapping to SDR displays.

Common formats include MP4, MKV, MOV, WMV, AVI, and MPEG-2 TS. Install additional VLC codecs if needed for rare formats.

The screensaver folder must include `libvlc.dll`, the `libvlc` native libraries, and the `plugins` directory (copied automatically when building from source).

## Features

- Video sizing: fit, fill, or center
- No live preview in the small monitor in Windows Screen Saver settings (use the **Preview** button for full-screen test)
- Control audio volume or mute audio
- HDR to SDR tone mapping (vivid colors on standard monitors, similar to VLC)

## Installation

To install, run the installer:

[Download for Windows](https://github.com/tonyfederer/EasyVideoScreensaver/releases/download/v1.2/EasyVideoScreensaverSetup.zip)

After installing, you can select Easy Video Screensaver as your active screensaver 
using these steps:

### Windows 10:

1. Go to Start > Settings.
2. Click Personalization.
3. Click Lock Screen.
4. Click Screen saver settings.

### Windows 7/8/8.1

1. Go to Start > Control Panel.
2. Click Appearance and Personalization.
3. Click Personalization.
4. Click Screen Saver.

## Feedback/Contribute

You can view the source code, report issues, and contribute on [Github](https://github.com/tonyfederer/EasyVideoScreensaver).

## Donate

If you find this project useful, please consider donating.  Your donations are appreciated. =)

[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=DC96GRBH4877Q)

## Third-Party Components

Video playback uses LibVLC (LGPL v2.1). See the [LibVLCSharp](https://github.com/videolan/libvlcsharp) and [VideoLAN](https://www.videolan.org/) projects for license details.

## Version History

### Unreleased
- Replaced WPF MediaElement with LibVLC for HDR to SDR tone mapping and broader format support
- Disabled embedded mini-preview in Screen Saver settings for faster dialog performance

### 1.2
- Added setting to resume video from previous position
- Hide mouse cursor when screensaver is playing
- Fixed error if video does not exist

### 1.1
- Upgraded to .NET 4.8
- Play video on all monitors
- Center settings window in screen

### 1.0
- Initial Release

## Copyright

Easy Video Screensaver is copyright (c) 2020 by [Tony Federer](https://github.com/tonyfederer) and released under the MIT License.