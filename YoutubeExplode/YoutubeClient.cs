// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <YoutubeClient.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    public partial class YoutubeClient
    {
        /// <summary>
        /// Delegate that (synchronously) handles GET requests and returns the content of the page as a string
        /// </summary>
        /// <param name="url">URL of the request</param>
        /// <returns>The page's HTML content as string or null if the operation failed</returns>
        public delegate string PerformGetRequestDelegate(string url);

        /// <summary>
        /// Default instance of YoutubeClient
        /// </summary>
        public static YoutubeClient Default { get; } = new YoutubeClient();

        private static string PerformGetRequestDefault(string url)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    return client.DownloadString(url);
                }
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// YoutubeClient
    /// </summary>
    public partial class YoutubeClient
    {
        private static readonly Regex VideoUrlToIDRegex = new Regex(@"[?&]v=(.+?)(?:&|$)",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string Protocol => UseSSL ? "https://" : "http://";

        /// <summary>
        /// Delegate that handles GET request for the client
        /// </summary>
        public PerformGetRequestDelegate GetRequestDelegate { get; set; }

        /// <summary>
        /// Whether to use the HTTPS protocol instead of HTTP for requests
        /// </summary>
        public bool UseSSL { get; set; } = true;

        /// <inheritdoc />
        public YoutubeClient()
        {
            GetRequestDelegate = PerformGetRequestDefault;
        }

        /// <summary>
        /// Get full information about a video by its ID
        /// </summary>
        /// <param name="videoID">The ID of the video</param>
        /// <param name="decipherIfNeeded">When set to true, videos with encrypted signatures will be automatically deciphered. This requires one extra HTTP request and some computational time. If set to false, the <see cref="VideoInfo"/> will need to be deciphered manually using <see cref="Decipher"/> method. Non-deciphered <see cref="VideoInfo"/> objects are still fully usable, but it will not be possible to access its <see cref="VideoStreamEndpoint"/> by URL</param>
        /// <returns><see cref="VideoInfo"/> object with the information on the given video</returns>
        public VideoInfo GetVideoInfo(string videoID, bool decipherIfNeeded = true)
        {
            if (videoID.IsBlank())
                throw new ArgumentNullException(nameof(videoID));

            // Grab info
            string url = $"{Protocol}youtube.com/watch?v={videoID}";
            string response = GetRequestDelegate(url);
            if (response.IsBlank())
                throw new Exception("Could not get video info");

            // Parse
            var result = Parser.ParseVideoInfo(response);
            if (result == null)
                throw new Exception("Could not parse video info");

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
                throw new ArgumentException("Given video info does not need to be deciphered", nameof(videoInfo));
            if (videoInfo.PlayerVersion.IsBlank())
                throw new Exception("Given video info does not have information about the player version");

            // Get the javascript source URL
            string url = $"{Protocol}s.ytimg.com/yts/jsbin/player-{videoInfo.PlayerVersion}/base.js";
            string response = GetRequestDelegate(url);
            if (response.IsBlank())
                throw new Exception("Could not get the video player source code");

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
            var match = VideoUrlToIDRegex.Match(videoURL);
            if (match.Success)
                videoID = match.Groups[1].Value;
            return match.Success;
        }
    }
}