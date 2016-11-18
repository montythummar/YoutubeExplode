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
        /// Pixel resolution of the video stream (raw string)
        /// </summary>
        public string ResolutionString { get; internal set; }

        /// <summary>
        /// Video bitrate
        /// </summary>
        public int Bitrate { get; internal set; }

        /// <summary>
        /// Frame update frequency of this video
        /// </summary>
        public double FPS { get; internal set; }

        /// <summary>
        /// Type of the video stream.
        /// This is a computed property.
        /// </summary>
        public VideoStreamType Type
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TypeString)) return VideoStreamType.Unknown;

                if (TypeString.IndexOf("video/mp4", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.MP4;
                if (TypeString.IndexOf("video/webm", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.WebM;
                if (TypeString.IndexOf("video/3gpp", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.ThirdGenerationPartnershipProject;
                if (TypeString.IndexOf("audio/mp4", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.AudioOnlyMP4;
                if (TypeString.IndexOf("audio/webm", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.AudioOnlyWebM;
                if (TypeString.IndexOf("audio/3gpp", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamType.AudioOnlyThirdGenerationPartnershipProject;

                return VideoStreamType.Unknown;
            }
        }

        /// <summary>
        /// Quality of the video stream.
        /// This is a computed property.
        /// </summary>
        public VideoStreamQuality Quality
        {
            get
            {
                if (Type == VideoStreamType.AudioOnlyMP4 ||
                    Type == VideoStreamType.AudioOnlyWebM ||
                    Type == VideoStreamType.AudioOnlyThirdGenerationPartnershipProject)
                    return VideoStreamQuality.NoVideo;

                if (string.IsNullOrWhiteSpace(QualityString)) return VideoStreamQuality.Unknown;

                if (QualityString.IndexOf("1080p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.High1080;
                if (QualityString.IndexOf("720p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.High720;
                if (QualityString.IndexOf("hd", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.High;
                if (QualityString.IndexOf("480p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.Medium480;
                if (QualityString.IndexOf("360p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.Medium360;
                if (QualityString.IndexOf("medium", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.Medium;
                if (QualityString.IndexOf("240p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.Low240;
                if (QualityString.IndexOf("144p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.Low144;
                if (QualityString.IndexOf("small", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return VideoStreamQuality.Low;

                return VideoStreamQuality.Unknown;
            }
        }

        /// <summary>
        /// File extension of the video file, based on its type.
        /// This is a computed property.
        /// </summary>
        public string FileExtension
        {
            get
            {
                if (Type == VideoStreamType.MP4 || Type == VideoStreamType.AudioOnlyMP4)
                    return "mp4";
                if (Type == VideoStreamType.WebM || Type == VideoStreamType.AudioOnlyWebM)
                    return "webm";
                if (Type == VideoStreamType.ThirdGenerationPartnershipProject || Type == VideoStreamType.AudioOnlyThirdGenerationPartnershipProject)
                    return "3gpp";

                // Default is mp4
                return "mp4";
            }
        }

        internal VideoStreamEndpoint() { }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Type} | {Quality}";
        }
    }
}