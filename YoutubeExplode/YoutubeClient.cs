// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <YoutubeClient.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Net;
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
        /// Delegate that handles GET requests and returns the content of the page as a string
        /// </summary>
        /// <param name="url">URL of the request</param>
        /// <returns>The page HTML content as string</returns>
        public delegate string PerformGetRequestDelegate(string url);

        private static readonly Regex VideoUrlToIDRegex = new Regex("[?&]v=(.+?)(?:&|$)",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex PlayerJavascriptSourceRegex = new Regex("\"js\"\\s?:\\s?\"(.+?)\"",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Default instance of YoutubeClient
        /// </summary>
        public static YoutubeClient Default { get; } = new YoutubeClient();

        private static string PerformGetRequestDefault(string url)
        {
            try
            {
                using (var client = new WebClient())
                    return client.DownloadString(url);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Delegate that handles GET request for the client
        /// </summary>
        public PerformGetRequestDelegate GetRequestDelegate { get; set; }

        /// <inheritdoc />
        public YoutubeClient()
        {
            GetRequestDelegate = PerformGetRequestDefault;
        }

        /// <summary>
        /// Get full information about a video by its ID
        /// </summary>
        /// <param name="videoID">The ID of the video</param>
        /// <param name="decipherIfNeeded">When set to true, videos with encrypted signatures will be automatically deciphered. This requires extra HTTP requests and some computational time. If set to false, the <see cref="VideoInfo"/> will need to be deciphered manually using <see cref="Decipher"/> method. Non-deciphered <see cref="VideoInfo"/> objects are still fully usable, but it will not be possible to access its <see cref="VideoStreamEndpoint"/> by URL</param>
        /// <returns><see cref="VideoInfo"/> object with the information on the given video</returns>
        public VideoInfo GetVideoInfo(string videoID, bool decipherIfNeeded = true)
        {
            if (string.IsNullOrWhiteSpace(videoID))
                throw new ArgumentException("Video ID should not be null or empty", nameof(videoID));

            // Grab info
            string url = $"http://youtube.com/get_video_info?video_id={videoID}";
            string response = GetRequestDelegate(url);
            if (string.IsNullOrWhiteSpace(response))
                throw new Exception($"Could not get video info for {videoID}");

            // Parse
            var result = Parser.ParseVideoInfo(response);
            if (result == null)
                throw new Exception($"Could not parse video info for {videoID}");

            // Decipher
            if (result.NeedsDeciphering && decipherIfNeeded)
                Decipher(result);

            return result;
        }

        /// <summary>
        /// Deciphers the streams (if required)
        /// </summary>
        public void Decipher(VideoInfo videoInfo)
        {
            if (!videoInfo.NeedsDeciphering)
                throw new ArgumentException("Given video info does not require to be deciphered", nameof(videoInfo));

            // Get the javascript source URL
            string url = $"http://youtube.com/watch?v={videoInfo.ID}";
            string response = GetRequestDelegate(url);
            if (string.IsNullOrWhiteSpace(response))
                throw new Exception($"Could not get video page for {videoInfo.ID}");

            // Look for the required string
            var match = PlayerJavascriptSourceRegex.Match(response);
            if (!match.Success)
                throw new Exception($"Could not parse video page for {videoInfo.ID}");
            string jsUrl = match.Groups[1].Value.Replace("\\", string.Empty);
            jsUrl = jsUrl.ToUri("http://youtube.com").AbsoluteUri;

            // Get the js
            response = GetRequestDelegate(jsUrl);

            // Decipher
            Decipherer.Decipher(videoInfo, response);
        }

        /// <summary>
        /// Parses video ID from a youtube video URL
        /// </summary>
        /// <returns>Video ID</returns>
        public string ParseVideoID(string videoURL)
        {
            var match = VideoUrlToIDRegex.Match(videoURL);
            if (!match.Success)
                throw new FormatException($"Could not parse Video ID from given string: {videoURL}");
            return match.Groups[1].Value;
        }

        /// <summary>
        /// Tries to parse video ID from a youtube video URL
        /// </summary>
        /// <returns>Whether the execution was successful or not</returns>
        public bool TryParseVideoID(string videoURL, out string videoID)
        {
            videoID = default(string);
            var match = VideoUrlToIDRegex.Match(videoURL);
            if (match.Success)
                videoID = match.Groups[1].Value;
            return match.Success;
        }
    }
}