// ReSharper disable InconsistentNaming (File extensions)

namespace YoutubeExplode.Models
{
    /// <summary>
    /// Type of a a video stream
    /// </summary>
    public enum VideoStreamType
    {
        /// <summary>
        /// Video type could not be identified
        /// </summary>
        Unknown,

        /// <summary>
        /// MPEG-4 Part 14 (.mp4) video stream
        /// </summary>
        MP4,

        /// <summary>
        /// WebM (.webm) video stream
        /// </summary>
        WebM,

        /// <summary>
        /// 3rd Generation Partnership Project (.3gpp) video stream
        /// </summary>
        TGPP,

        /// <summary>
        /// Flash video (.flv) video stream
        /// </summary>
        FLV,

        /// <summary>
        /// Transport stream video (.ts) video stream
        /// </summary>
        TS
    }
}