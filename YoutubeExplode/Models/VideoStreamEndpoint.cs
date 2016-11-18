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
        private static VideoStreamType ParseType(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString)) return VideoStreamType.Unknown;

            if (typeString.IndexOf("video/mp4", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamType.MP4;
            if (typeString.IndexOf("video/webm", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamType.WebM;
            if (typeString.IndexOf("video/3gpp", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamType.ThirdGenerationPartnershipProject;
            if (typeString.IndexOf("audio/mp4", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamType.AudioOnlyMP4;
            if (typeString.IndexOf("audio/webm", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamType.AudioOnlyWebM;
            if (typeString.IndexOf("audio/3gpp", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamType.AudioOnlyThirdGenerationPartnershipProject;

            return VideoStreamType.Unknown;
        }

        private static VideoStreamQuality ParseQuality(string qualityString)
        {
            if (string.IsNullOrWhiteSpace(qualityString)) return VideoStreamQuality.Unknown;

            if (qualityString.IndexOf("1080p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.High1080;
            if (qualityString.IndexOf("720p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.High720;
            if (qualityString.IndexOf("hd", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.High;
            if (qualityString.IndexOf("480p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.Medium480;
            if (qualityString.IndexOf("360p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.Medium360;
            if (qualityString.IndexOf("medium", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.Medium;
            if (qualityString.IndexOf("240p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.Low240;
            if (qualityString.IndexOf("144p", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.Low144;
            if (qualityString.IndexOf("small", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return VideoStreamQuality.Low;

            return VideoStreamQuality.Unknown;
        }

        private static string GetExtension(VideoStreamType type)
        {
            if (type == VideoStreamType.MP4 || type == VideoStreamType.AudioOnlyMP4)
                return "mp4";
            if (type == VideoStreamType.WebM || type == VideoStreamType.AudioOnlyWebM)
                return "webm";
            if (type == VideoStreamType.ThirdGenerationPartnershipProject || type == VideoStreamType.AudioOnlyThirdGenerationPartnershipProject)
                return "3gpp";

            // Default is mp4
            return "mp4";
        }

        private string _typeString;
        private string _qualityString;
        private VideoStreamType _type;

        /// <summary>
        /// URL of the stream
        /// </summary>
        public string URL { get; internal set; }

        /// <summary>
        /// Type of the video stream (raw string)
        /// </summary>
        public string TypeString
        {
            get { return _typeString; }
            internal set
            {
                _typeString = value;
                Type = ParseType(value);
            }
        }

        /// <summary>
        /// Quality of the video stream (raw string)
        /// </summary>
        public string QualityString
        {
            get { return _qualityString; }
            internal set
            {
                _qualityString = value;

                if (Type == VideoStreamType.AudioOnlyMP4 ||
                    Type == VideoStreamType.AudioOnlyWebM ||
                    Type == VideoStreamType.AudioOnlyThirdGenerationPartnershipProject)
                    Quality = VideoStreamQuality.NoVideo;
                else
                    Quality = ParseQuality(value);
            }
        }

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
        /// Type of the video stream
        /// </summary>
        public VideoStreamType Type
        {
            get { return _type; }
            private set
            {
                _type = value;
                FileExtension = GetExtension(value);
            }
        }

        /// <summary>
        /// Quality of the video stream
        /// </summary>
        public VideoStreamQuality Quality { get; private set; }

        /// <summary>
        /// File extension of the video file, based on its type
        /// </summary>
        public string FileExtension { get; private set; }

        internal VideoStreamEndpoint() { }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Type} | {Quality}";
        }
    }
}