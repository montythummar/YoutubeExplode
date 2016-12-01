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
        /// Whether this video is listed publicly
        /// </summary>
        public bool IsListed { get; internal set; }

        /// <summary>
        /// Whether it's allowed to leave user rating on this video
        /// </summary>
        public bool IsRatingAllowed { get; internal set; }

        /// <summary>
        /// Whether the audio of this video has been muted
        /// </summary>
        public bool IsMuted { get; internal set; }

        /// <summary>
        /// Whether it's allowed to embed this video outside of youtube
        /// </summary>
        public bool IsEmbeddingAllowed { get; internal set; }

        /// <summary>
        /// Whether this video has closed captions
        /// </summary>
        public bool HasClosedCaptions { get; internal set; }

        /// <summary>
        /// View count
        /// </summary>
        public ulong ViewCount { get; internal set; }

        /// <summary>
        /// Average user rating
        /// </summary>
        public double AverageRating { get; internal set; }

        /// <summary>
        /// Video keywords used for searching
        /// </summary>
        public string[] Keywords { get; internal set; }

        /// <summary>
        /// Video streams
        /// </summary>
        public VideoStreamEndpoint[] Streams { get; internal set; }

        /// <summary>
        /// Version of the video player, used for this video's playback
        /// </summary>
        public string PlayerVersion { get; internal set; }

        /// <summary>
        /// Whether this video uses an encrypted signature for its streams that needs to be deciphered before the streams can be accessed
        /// </summary>
        public bool NeedsDeciphering { get; internal set; }

        internal VideoInfo() { }
    }
}