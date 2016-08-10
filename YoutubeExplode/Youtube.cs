// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <Youtube.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    /// <summary>
    /// Entry point for YoutubeExplode library
    /// </summary>
    public static class Youtube
    {
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// Get full information about a video by its ID
        /// </summary>
        /// <returns>Object of type <see cref="VideoInfo"/> with the information on the given video</returns>
        public static VideoInfo GetVideoInfo(string videoID)
        {
            // Check arguments
            if (string.IsNullOrWhiteSpace(videoID))
                throw new ArgumentException("Video ID should not be null or empty", nameof(videoID));

            // Grab info
            string url = $"http://youtube.com/get_video_info?video_id={videoID}";
            string rawInfo = Client.Get(url);
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