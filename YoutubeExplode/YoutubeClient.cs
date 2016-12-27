// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <YoutubeClient.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Text.RegularExpressions;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    /// <summary>
    /// YoutubeClient
    /// </summary>
    public class YoutubeClient
    {
        /// <summary>
        /// Default instance of YoutubeClient
        /// </summary>
        public static YoutubeClient Default { get; } = new YoutubeClient();

        /// <summary>
        /// HTTP request handler
        /// </summary>
        public IRequestHandler RequestHandler { get; set; } = DefaultRequestHandler.Default;

        /// <summary>
        /// Get full information about a video by its ID
        /// </summary>
        /// <param name="videoID">The ID of the video</param>
        /// <param name="decipherIfNeeded">When set to true, videos with encrypted signatures will be automatically deciphered. This requires one extra HTTP GET request and some computational time. If set to false, the <see cref="VideoInfo"/> will need to be deciphered manually using <see cref="Decipher"/> method. Non-deciphered <see cref="VideoInfo"/> objects are still fully usable, but it will not be possible to access its <see cref="VideoStreamEndpoint"/> by URL</param>
        /// <param name="getFileSizes">When set to true, it will also attempt to get file sizes of all obtained streams. This requires one extra HTTP HEAD request. If set to false, you can use <see cref="GetStreamFileSize"/> to get file size of individual streams. This parameter requires <paramref name="decipherIfNeeded"/> to be set.</param>
        /// <returns><see cref="VideoInfo"/> object with the information on the given video</returns>
        public VideoInfo GetVideoInfo(string videoID, bool decipherIfNeeded = true, bool getFileSizes = true)
        {
            if (videoID.IsBlank())
                throw new ArgumentNullException(nameof(videoID));

            // Grab watch page html code
            string url = $"https://youtube.com/watch?v={videoID}";
            string html = RequestHandler.GetHtml(url);
            if (html.IsBlank())
                throw new Exception("Could not get video watch page (GET request failed)");

            VideoInfo result;

            // Try to get video info
            var jsonMatch = Regex.Match(html, @"ytplayer\.config\s*=\s*(\{.+?\});",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (jsonMatch.Success)
            {
                // Get it directly from JSON object in the watch page
                result = Parser.ParseVideoInfoJson(jsonMatch.Groups[1].Value);
                if (result == null)
                    throw new Exception("Could not parse video info (JSON)");
            }
            else
            {
                // Get it from URL encoded data from internal api
                url = $"https://youtube.com/get_video_info?video_id={videoID}";
                html = RequestHandler.GetHtml(url);
                if (html.IsBlank())
                    throw new Exception("Could not get URL-encoded video info (GET request failed)");
                result = Parser.ParseVideoInfoUrlEncoded(html);
                if (result == null)
                    throw new Exception("Could not parse video info (URL-encoded)");
            }

            // Decipher
            if (result.NeedsDeciphering && decipherIfNeeded)
                Decipher(result);

            // Get file size of streams
            if (getFileSizes)
            {
                foreach (var stream in result.Streams)
                    GetStreamFileSize(stream);
            }

            return result;
        }

        /// <summary>
        /// Deciphers the streams in the given <see cref="VideoInfo"/>
        /// </summary>
        public void Decipher(VideoInfo videoInfo)
        {
            if (!videoInfo.NeedsDeciphering)
                throw new ArgumentException("Given video info does not need to be deciphered", nameof(videoInfo));
            if (videoInfo.PlayerVersion.IsBlank())
                throw new Exception("Given video info does not have information about the player version");

            // Get the javascript source URL
            string url = $"https://s.ytimg.com/yts/jsbin/player-{videoInfo.PlayerVersion}/base.js";
            string response = RequestHandler.GetHtml(url);
            if (response.IsBlank())
                throw new Exception("Could not get the video player source code");

            // Decipher
            Decipherer.Decipher(videoInfo, response);
        }

        /// <summary>
        /// Gets the file size of the video, streamed on given endpoint
        /// </summary>
        public ulong GetStreamFileSize(VideoStreamEndpoint stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (stream.URL.IsBlank())
                throw new Exception("Given stream does not have an URL");
            if (stream.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            // Get the headers
            var headers = RequestHandler.GetHeaders(stream.URL);
            if (headers == null)
                throw new Exception("Could not obtain headers");

            return stream.FileSize = headers.GetValueOrDefault("Content-Length").ParseUlongOrDefault();
        }

        /// <summary>
        /// Parses video ID from a youtube video URL
        /// </summary>
        /// <returns>Video ID</returns>
        public string ParseVideoID(string videoURL)
        {
            var match = Regex.Match(videoURL, @"[?&]v=(.+?)(?:&|$)",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new FormatException("Could not parse Video ID from given string");
            return match.Groups[1].Value;
        }

        /// <summary>
        /// Tries to parse video ID from a youtube video URL
        /// </summary>
        /// <returns>Whether the execution was successful or not</returns>
        public bool TryParseVideoID(string videoURL, out string videoID)
        {
            videoID = default(string);
            var match = Regex.Match(videoURL, @"[?&]v=(.+?)(?:&|$)",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            if (match.Success)
                videoID = match.Groups[1].Value;
            return match.Success;
        }
    }
}