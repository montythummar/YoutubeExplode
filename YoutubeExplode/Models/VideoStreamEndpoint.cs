// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <VideoStreamEndpoint.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

namespace YoutubeExplode.Models
{
    /// <summary>
    /// Stream endpoint for a Youtube video
    /// </summary>
    public class VideoStreamEndpoint
    {
        #region Static
        private static VideoStreamType ParseType(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString)) return VideoStreamType.Unknown;

            if (typeString.ContainsInvariant("video/mp4"))
                return VideoStreamType.MP4;
            if (typeString.ContainsInvariant("video/webm"))
                return VideoStreamType.WebM;
            if (typeString.ContainsInvariant("video/3gpp"))
                return VideoStreamType.ThirdGenerationPartnershipProject;
            if (typeString.ContainsInvariant("audio/mp4"))
                return VideoStreamType.AudioOnlyMP4;
            if (typeString.ContainsInvariant("audio/webm"))
                return VideoStreamType.AudioOnlyWebM;
            if (typeString.ContainsInvariant("audio/3gpp"))
                return VideoStreamType.AudioOnlyThirdGenerationPartnershipProject;

            return VideoStreamType.Unknown;
        }

        private static VideoStreamQuality ParseQuality(string qualityString)
        {
            if (string.IsNullOrWhiteSpace(qualityString)) return VideoStreamQuality.Unknown;

            if (qualityString.ContainsInvariant("1080p"))
                return VideoStreamQuality.High1080;
            if (qualityString.ContainsInvariant("720p") || qualityString.ContainsInvariant("hd720"))
                return VideoStreamQuality.High720;
            if (qualityString.ContainsInvariant("hd"))
                return VideoStreamQuality.High;
            if (qualityString.ContainsInvariant("480p"))
                return VideoStreamQuality.Medium480;
            if (qualityString.ContainsInvariant("360p"))
                return VideoStreamQuality.Medium360;
            if (qualityString.ContainsInvariant("medium"))
                return VideoStreamQuality.Medium;
            if (qualityString.ContainsInvariant("240p"))
                return VideoStreamQuality.Low240;
            if (qualityString.ContainsInvariant("144p"))
                return VideoStreamQuality.Low144;
            if (qualityString.ContainsInvariant("small"))
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
        #endregion

        private string _typeString;
        private string _qualityString;
        private VideoStreamType _type;

        /// <summary>
        /// Video signature
        /// </summary>
        public string Signature { get; internal set; }

        /// <summary>
        /// Whether or not this stream's signature needs to be deciphered before the stream can be accessed
        /// </summary>
        public bool NeedsDeciphering { get; internal set; }

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
        /// Video bitrate (bits per second)
        /// </summary>
        public ulong Bitrate { get; internal set; }

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

        /// <summary>
        /// File size (in bytes) of the video
        /// </summary>
        public ulong FileSize { get; internal set; }

        internal VideoStreamEndpoint() { }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Type} | {Quality}";
        }
    }
}