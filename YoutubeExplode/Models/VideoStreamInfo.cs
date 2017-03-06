using YoutubeExplode.Internal;

namespace YoutubeExplode.Models
{
    /// <summary>
    /// Youtube video stream meta data
    /// </summary>
    public class VideoStreamInfo
    {
        /// <summary>
        /// URL of the stream
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// Signature
        /// </summary>
        public string Signature { get; internal set; }

        /// <summary>
        /// Whether the signature needs to be deciphered before manifest can be accessed by URL
        /// </summary>
        public bool NeedsDeciphering { get; internal set; }

        /// <summary>
        /// Internal type id
        /// </summary>
        public int Itag { get; internal set; }

        /// <summary>
        /// Adaptive mode
        /// </summary>
        public VideoStreamAdaptiveMode AdaptiveMode => ItagHelper.GetAdaptiveMode(Itag);

        /// <summary>
        /// Type of the video stream
        /// </summary>
        public VideoStreamType Type => ItagHelper.GetType(Itag);

        /// <summary>
        /// Quality of the video stream
        /// </summary>
        public VideoStreamQuality Quality => ItagHelper.GetQuality(Itag);

        /// <summary>
        /// Whether this video is a 3D video
        /// </summary>
        public bool Is3D => ItagHelper.GetIs3D(Itag);

        /// <summary>
        /// Whether this video is a live stream
        /// </summary>
        public bool IsLiveStream => ItagHelper.GetIsLiveStream(Itag);

        /// <summary>
        /// Video resolution.
        /// Some streams may not have this property.
        /// </summary>
        public VideoStreamResolution Resolution { get; internal set; }

        /// <summary>
        /// Video bitrate (bits per second).
        /// Some streams may not have this property.
        /// </summary>
        public long Bitrate { get; internal set; }

        /// <summary>
        /// Frame update frequency of this video.
        /// Some streams may not have this property.
        /// </summary>
        public double Fps { get; internal set; }

        /// <summary>
        /// Quality label
        /// </summary>
        public string QualityLabel => ItagHelper.GetQualityLabel(Quality);

        /// <summary>
        /// File extension of the video file, based on its type
        /// </summary>
        public string FileExtension => ItagHelper.GetExtension(Type);

        /// <summary>
        /// File size (in bytes) of the video
        /// </summary>
        public long FileSize { get; internal set; }

        internal VideoStreamInfo() { }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{AdaptiveMode} | {Type} | {Quality}";
        }
    }
}