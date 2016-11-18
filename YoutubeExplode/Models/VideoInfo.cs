// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <VideoInfo.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;

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
        /// URL for the highest thumbnail
        /// </summary>
        public string Thumbnail { get; internal set; }

        /// <summary>
        /// URL for the highest res image
        /// </summary>
        public string ImageHighQuality { get; internal set; }

        /// <summary>
        /// URL for the medium res image
        /// </summary>
        public string ImageMediumQuality { get; internal set; }

        /// <summary>
        /// URL for the lowest res image
        /// </summary>
        public string ImageLowQuality { get; internal set; }

        /// <summary>
        /// Collection of watermark URLs
        /// </summary>
        public string[] Watermarks { get; internal set; }

        /// <summary>
        /// Length of the video
        /// </summary>
        public TimeSpan Length { get; internal set; }

        /// <summary>
        /// Is the video publicly listed?
        /// </summary>
        public bool IsListed { get; internal set; }

        /// <summary>
        /// Is it allowed to rate the video?
        /// </summary>
        public bool IsRatingAllowed { get; internal set; }

        /// <summary>
        /// Is the video's audio muted?
        /// </summary>
        public bool IsMuted { get; internal set; }

        /// <summary>
        /// Is it allowed to embed this video?
        /// </summary>
        public bool IsEmbedingAllowed { get; internal set; }

        /// <summary>
        /// Does this video have closed captions available?
        /// </summary>
        public bool HasClosedCaptions { get; internal set; }

        /// <summary>
        /// View count
        /// </summary>
        public int ViewCount { get; internal set; }

        /// <summary>
        /// Average user rating
        /// </summary>
        public double AvgRating { get; internal set; }

        /// <summary>
        /// Video keywords used for searching
        /// </summary>
        public string[] Keywords { get; internal set; }

        /// <summary>
        /// Does this video use an encrypted signature?
        /// </summary>
        internal bool UseCipherSignature { get; set; }

        /// <summary>
        /// Video streams
        /// </summary>
        public VideoStreamEndpoint[] Streams { get; internal set; }

        internal VideoInfo() { }
    }
}