YoutubeExplode
===================


Light-weight .NET library that parses public meta data on Youtube videos.


**Download:**

The library is distributed as a [nuget package](https://www.nuget.org/packages/YoutubeExplode): `Install-Package YoutubeExplode`

You can also find the last stable version in [releases](https://github.com/Tyrrrz/YoutubeExplode/releases)

**Features:**

- Gets video meta and video streams meta
- Supports video meta data from different sources: watch page, internal API
- Supports stream meta data from different sources: non-adaptive, adaptive embedded, adaptive dash
- Meta data properties are exposed via enums and other strong types
- Deciphers signatures for video streams automatically or on-demand
- Gets file sizes for video streams automatically or on-demand
- Downloads video to stream or to file
- Exposes _async_ API as well as standard synchronous API without wrappers
- Exposes static methods to extract video ID from URL and to validate video ID
- Allows substituting default HTTP handler
- Caches decompiled player sources to increase performance
- XML documentation for every public member

**Parsed meta data:**

 - Video id, title and author
 - Length
 - View count
 - Rating
 - Search keywords
 - URLs of thumbnail, high/medium/low quality images
 - URLS of watermarks
 - Is listed, is muted, allows ratings, allows embedding, has closed captions
 - Video streams
 - Caption tracks

Video stream objects include the following meta data:

 - URL
 - Type
 - Quality
 - Resolution
 - Bitrate
 - FPS
 - File size

Caption track objects include the following meta data:

 - URL
 - Language

**Usage:**

Check out `YoutubeExplodeDemoConsole` or `YoutubeExplodeDemoWpf` projects for real examples.

```c#
using System;
using System.Linq;
using YoutubeExplode;

// Get client instance
var client = new YoutubeClient();
//       ... YoutubeClient.Instance;

// Get info
var videoInfo = client.GetVideoInfo("bx_KorIwABQ");
//          ... await client.GetVideoInfoAsync("bx_KorIwABQ");

// Output some of it to console
Console.WriteLine($"Title: {videoInfo.Title}");
Console.WriteLine($"Author: {videoInfo.Author}");
Console.WriteLine($"Length: {videoInfo.Length}");

// Download the first video stream to file
var streamInfo = videoInfo.Streams.First();
string fileName = $"{videoInfo.Id}_{streamInfo.Quality}.{streamInfo.FileExtension}";
client.DownloadVideo(streamInfo, fileName);
// await client.DownloadVideoAsync(streamInfo, fileName);

```

**Libraries used:**

- [SimpleJson](https://github.com/facebook-csharp-sdk/simple-json) - lightweight JSON parser
- [GalaSoft.MVVMLight](http://www.mvvmlight.net) - MVVM rapid development
- [MaterialDesignXAML](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit) - MaterialDesign UI
- [Tyrrrz.Extensions](https://github.com/Tyrrrz/Extensions) - my set of various extensions for rapid development
- [Tyrrrz.WpfExtensions](https://github.com/Tyrrrz/WpfExtensions) - my set of various WPF extensions for rapid development
 
**Screenshots:**

![](http://www.tyrrrz.me/projects/images/ytexplode_1.png)
![](http://www.tyrrrz.me/projects/images/ytexplode_2.png)
