namespace YoutubeExplode.Models
{
    /// <summary>
    /// QualityString of a video stream
    /// </summary>
    public enum VideoStreamQuality
    {
        /// <summary>
        /// Video quality could not be identified
        /// </summary>
        Unknown,

        /// <summary>
        /// There is no video in this stream (audio-only)
        /// </summary>
        NoVideo,

        /// <summary>
        /// Low-quality video stream with unspecified resolution
        /// </summary>
        Low,

        /// <summary>
        /// 144p low-quality video stream
        /// </summary>
        Low144,

        /// <summary>
        /// 240p low-quality video stream
        /// </summary>
        Low240,

        /// <summary>
        /// Medium-quality video stream with unspecified resolution
        /// </summary>
        Medium,

        /// <summary>
        /// 360p medium-quality video stream
        /// </summary>
        Medium360,

        /// <summary>
        /// 480p medium-quality video stream
        /// </summary>
        Medium480,

        /// <summary>
        /// HD video stream with unspecified resolution
        /// </summary>
        High,

        /// <summary>
        /// 720p HD video stream
        /// </summary>
        High720,

        /// <summary>
        /// 1080p HD video stream
        /// </summary>
        High1080,

        /// <summary>
        /// 1440p HD video stream
        /// </summary>
        High1440,

        /// <summary>
        /// 2160p (4K) HD video stream
        /// </summary>
        High2160
    }
}