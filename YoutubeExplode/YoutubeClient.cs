// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <YoutubeClient.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Net;
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
        /// <returns>Object of type <see cref="VideoInfo"/> with the information on the given video</returns>
        public VideoInfo GetVideoInfo(string videoID)
        {
            // Check arguments
            if (string.IsNullOrWhiteSpace(videoID))
                throw new ArgumentException("Video ID should not be null or empty", nameof(videoID));

            // Grab info
            string url = $"http://youtube.com/get_video_info?video_id={videoID}";
            string rawInfo = GetRequestDelegate(url);
            if (string.IsNullOrWhiteSpace(rawInfo))
                throw new Exception($"Could not download video info for {videoID}");

            // Parse
            var result = Parser.ParseVideoInfo(rawInfo);
            if (result == null)
                throw new Exception($"Could not parse video info for {videoID}");

            return result;
        }
    }
}