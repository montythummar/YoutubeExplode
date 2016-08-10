// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <VideoStreamEndpoint.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;

namespace YoutubeExplode.Models
{
    /// <summary>
    /// Stream endpoint for a Youtube video
    /// </summary>
    public class VideoStreamEndpoint
    {
        /// <summary>
        /// URL of the stream
        /// </summary>
        public string URL { get; internal set; }

        /// <summary>
        /// Type of the video stream (raw string)
        /// </summary>
        public string TypeString { get; internal set; }

        /// <summary>
        /// Quality of the video stream (raw string)
        /// </summary>
        public string QualityString { get; internal set; }

        /// <summary>
        /// Type of the video stream
        /// </summary>
        public VideoStreamType Type
        {
            get
            {
                if (TypeString.IndexOf("video/mp4", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.MP4;
                if (TypeString.IndexOf("video/webm", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.WebM;
                if (TypeString.IndexOf("video/3gpp", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.ThirdGenerationPartnershipProject;
                return VideoStreamType.Unknown;
            }
        }

        /// <summary>
        /// Quality of the video stream
        /// </summary>
        public VideoStreamQuality Quality
        {
            get
            {
                if (QualityString.IndexOf("hd", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.High;
                if (QualityString.IndexOf("medium", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.Medium;
                if (QualityString.IndexOf("small", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.Low;
                return VideoStreamQuality.Unknown;
            }
        }

        /// <summary>
        /// File extension of the video file, based on its type
        /// </summary>
        public string FileExtension
        {
            get
            {
                if (Type == VideoStreamType.MP4)
                    return "mp4";
                if (Type == VideoStreamType.WebM)
                    return "webm";
                if (Type == VideoStreamType.ThirdGenerationPartnershipProject)
                    return "3gpp";

                // Default is mp4
                return "mp4";
            }
        }

        internal VideoStreamEndpoint() { }
    }
}