YoutubeExplode
===================


Zero-dependency .NET library that parses out various public information on Youtube videos, including download URLs. Works with signed videos.

**Parsed data:**

 - Video id, title and author
 - Search keywords
 - Length
 - View count
 - Rating
 - URLs of thumbnail, high/medium/low quality images
 - URLS of watermarks
 - Whether this video is listed, is muted, allows ratings, allows embeding and has closed captions
 - Video streams

The video stream objects include the following data:

 - URL
 - Type
 - Quality
 - Resolution
 - Bitrate
 - FPS

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

**Screenshots:**

![](http://www.tyrrrz.me/projects/images/ytexplode_1.png)
![](http://www.tyrrrz.me/projects/images/ytexplode_2.png)
