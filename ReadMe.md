YoutubeExplode
===================


Light-weight .NET library that parses out various public information on Youtube videos, including download URLs. Works with signed videos.


**Download:**

The library is distributed as a [nuget package](https://www.nuget.org/packages/YoutubeExplode)

If you want to download the demo - head to the [releases](https://github.com/Tyrrrz/YoutubeExplode/releases) page

**Parsed data:**

 - Video id, title and author
 - Search keywords
 - Length
 - View count
 - Rating
 - URLs of thumbnail, high/medium/low quality images
 - URLS of watermarks
 - Whether this video is listed, is muted, allows ratings, allows embedding and has closed captions
 - Video streams

The video stream objects include the following data:

 - URL
 - Type
 - Quality
 - Resolution
 - Bitrate
 - FPS
 - File size

**Usage:**

```c#
using System;
using System.Diagnostics;
using YoutubeExplode;

...

// Get info
var videoInfo = new YoutubeClient().GetVideoInfo("bx_KorIwABQ");

// Output some of it to console
Console.WriteLine($"Title: {videoInfo.Title}");
Console.WriteLine($"Author: {videoInfo.Author}");
Console.WriteLine($"Length: {videoInfo.Length}");

// Open the first video stream in a browser window and watch the video
Process.Start(videoInfo.Streams.First().URL);

```

**Library dependencies:**
- [SimpleJSON](https://github.com/facebook-csharp-sdk/simple-json) - lightweight JSON parser


**Demo dependencies:**

 - [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) - this library
 - [GalaSoft.MVVMLight](http://www.mvvmlight.net) - MVVM rapid development
 - [MaterialDesignXAML](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit) - MaterialDesign UI
 - [Tyrrrz.Extensions](https://github.com/Tyrrrz/Extensions) - my set of various extensions for rapid development
 - [Tyrrrz.WpfExtensions](https://github.com/Tyrrrz/WpfExtensions) - my set of various WPF extensions for rapid development
 
**Screenshots:**

![](http://www.tyrrrz.me/projects/images/ytexplode_1.png)
![](http://www.tyrrrz.me/projects/images/ytexplode_2.png)
