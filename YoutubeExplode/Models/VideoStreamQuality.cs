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
        /// 2160p (4K) HD video stream
        /// </summary>
        High2160,

        /// <summary>
        /// 1440p HD video stream
        /// </summary>
        High1440,

        /// <summary>
        /// 1080p HD video stream
        /// </summary>
        High1080,

        /// <summary>
        /// 720p HD video stream
        /// </summary>
        High720,

        /// <summary>
        /// HD video stream with unspecified resolution
        /// </summary>
        High,

        /// <summary>
        /// 480p medium-quality video stream
        /// </summary>
        Medium480,

        /// <summary>
        /// 360p medium-quality video stream
        /// </summary>
        Medium360,

        /// <summary>
        /// Medium-quality video stream with unspecified resolution
        /// </summary>
        Medium,

        /// <summary>
        /// 240p low-quality video stream
        /// </summary>
        Low240,

        /// <summary>
        /// 144p low-quality video stream
        /// </summary>
        Low144,

        /// <summary>
        /// Low-quality video stream with unspecified resolution
        /// </summary>
        Low,
        
        /// <summary>
        /// There is no video in this stream (audio-only)
        /// </summary>
        NoVideo
    }
}