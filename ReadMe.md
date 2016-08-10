YoutubeExplode
===================


Zero-dependency .NET library that parses out various public information on Youtube videos, including download URLs

**Parsed data:**

 - Title (string)
 - Author (string)
 - Thumbnail URL (string)
 - Length (TimeSpan)
 - Is Listed (bool)
 - View Count (int)
 - Average User Rating (double)
 - Streaming Endpoints (IEnumerable\<VideoStreamEndpoint\>)
  - TypeString (string)
  - QualityString (string)
  - URL (string)
  - Type (enum => [Unknown, MP4, WebM, ThirdGenerationPartnershipProject])
  - Quality (enum => [Unknown, High, Medium, Low])
  - FileExtension (string)

**Usage:**

```c#
using System;
using System.Diagnostics;
using YoutubeExplode;

...

// Get info
var videoInfo = Youtube.GetVideoInfo("bx_KorIwABQ");

// Output some of it to console
Console.WriteLine($"Title: {videoInfo.Title}");
Console.WriteLine($"Author: {videoInfo.Author}");
Console.WriteLine($"Length: {videoInfo.Length}");

// Open the first video stream in a browser window and watch the video
Process.Start(videoInfo.Streams.First().URL);

```

**Screenshots:**

![](http://www.tyrrrz.me/projects/images/ytexplode_1.jpg)
