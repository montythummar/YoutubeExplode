YoutubeExplode
===================


Light-weight .NET library that parses out various public information on Youtube videos, including download URLs. Works with signed videos.


**Download:**

The library is distributed as a [nuget package](https://www.nuget.org/packages/YoutubeExplode): `Install-Package YoutubeExplode`

If you want to download the demo application - navigate to the [releases](https://github.com/Tyrrrz/YoutubeExplode/releases)

**Features:**

- Get video meta and video streams meta
- Strongly-typed enums for all applicable properties, along with raw strings
- Automatically or on-demand decipher signatures for video streams
- Automatically or on-demand get video file sizes
- Download video to stream or to file
- Support for both synchronous and asynchronous API wherever possible
- Methods to extract video ID from URL and to validate video ID
- Dependency injection for HTTP handler
- Fallbacks for legacy videos
- XML documentation

**Parsed meta data:**

 - Video id, title and author
 - Search keywords
 - Length
 - View count
 - Rating
 - URLs of thumbnail, high/medium/low quality images
 - URLS of watermarks
 - Whether this video is listed, is muted, allows ratings, allows embedding and has closed captions
 - Video streams

The video stream objects include the following meta data:

 - URL
 - Type
 - Quality
 - Resolution
 - Bitrate (bits/s)
 - FPS
 - File size (bytes)

**Usage:**

```c#
using System;
using System.Linq;
//using System.Threading.Tasks;
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

**Library dependencies:**
- [SimpleJson](https://github.com/facebook-csharp-sdk/simple-json) - lightweight JSON parser


**Demo dependencies:**

 - [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode) - this library
 - [GalaSoft.MVVMLight](http://www.mvvmlight.net) - MVVM rapid development
 - [MaterialDesignXAML](https://github.com/ButchersBoy/MaterialDesignInXamlToolkit) - MaterialDesign UI
 - [Tyrrrz.Extensions](https://github.com/Tyrrrz/Extensions) - my set of various extensions for rapid development
 - [Tyrrrz.WpfExtensions](https://github.com/Tyrrrz/WpfExtensions) - my set of various WPF extensions for rapid development
 
**Screenshots:**

![](http://www.tyrrrz.me/projects/images/ytexplode_1.png)
![](http://www.tyrrrz.me/projects/images/ytexplode_2.png)
