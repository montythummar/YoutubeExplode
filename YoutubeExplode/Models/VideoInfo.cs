// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <VideoInfo.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Collections.Generic;

namespace YoutubeExplode.Models
{
    /// <summary>
    /// Youtube Video info
    /// </summary>
    public class VideoInfo
    {
        /// <summary>
        /// Video ID
        /// </summary>
        public string ID { get; internal set; }

        /// <summary>
        /// Video title
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// Video author
        /// </summary>
        public string Author { get; internal set; }

        /// <summary>
        /// URL for the highest res thumbnail
        /// </summary>
        public string ThumbnailURL { get; internal set; }

        /// <summary>
        /// Length of the video
        /// </summary>
        public TimeSpan Length { get; internal set; }

        /// <summary>
        /// Is the video publicly listed?
        /// </summary>
        public bool IsListed { get; internal set; }

        /// <summary>
        /// Amount of views
        /// </summary>
        public int ViewCount { get; internal set; }

        /// <summary>
        /// Average user rating
        /// </summary>
        public double AvgRating { get; internal set; }

        /// <summary>
        /// Video streams
        /// </summary>
        public IEnumerable<VideoStreamEndpoint> Streams { get; internal set; }

        internal VideoInfo() { }
    }
}