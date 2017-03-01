using YoutubeExplode.Internal;

namespace YoutubeExplode.Models
{
    /// <summary>
    /// Stream endpoint for a Youtube video
    /// </summary>
    public partial class VideoStreamInfo
    {
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
        public string Url { get; internal set; }

        /// <summary>
        /// Internal type id
        /// </summary>
        public int Itag { get; internal set; }

        /// <summary>
        /// Whether this video contains video
        /// </summary>
        public bool HasVideo => GetHasVideo(Itag);

        /// <summary>
        /// Whether this video contains audio
        /// </summary>
        public bool HasAudio => GetHasAudio(Itag);

        /// <summary>
        /// Whether this video is a 3D video
        /// </summary>
        public bool Is3D => GetIs3D(Itag);

        /// <summary>
        /// Whether this video is a live stream
        /// </summary>
        public bool IsLiveStream => GetIsLiveStream(Itag);

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
        /// Type of the video stream
        /// </summary>
        public VideoStreamType Type => GetType(Itag);

        /// <summary>
        /// Quality of the video stream
        /// </summary>
        public VideoStreamQuality Quality => GetQuality(Itag);

        /// <summary>
        /// File extension of the video file, based on its type
        /// </summary>
        public string FileExtension => GetExtension(Type);

        /// <summary>
        /// File size (in bytes) of the video
        /// </summary>
        public long FileSize { get; internal set; }

        internal VideoStreamInfo() { }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Type} | {Quality}";
        }
    }

    public partial class VideoStreamInfo
    {
        private static bool GetHasVideo(int itag)
        {
            return itag.IsEither(17, 36, 5, 18, 22, 43, 82, 83, 84, 85, 100, 160, 133, 134, 135, 136, 298, 137, 299, 264,
                266, 138, 278, 242, 243, 244, 247, 248, 271, 313, 272, 302, 308, 315, 330, 331, 332, 333, 334, 335, 336,
                337, 91, 92, 93, 94, 95, 96);
        }

        private static bool GetHasAudio(int itag)
        {
            return itag.IsEither(17, 36, 5, 18, 22, 43, 82, 83, 84, 85, 100, 140, 141, 171, 249, 250, 251, 91, 92, 93,
                94, 95, 96, 127, 128);
        }

        private static bool GetIs3D(int itag)
        {
            return itag.IsEither(82, 83, 84, 85, 100, 101, 102);
        }

        private static bool GetIsLiveStream(int itag)
        {
            return itag.IsEither(91, 92, 93, 94, 95, 96, 127, 128);
        }

        private static VideoStreamType GetType(int itag)
        {
            if (itag.IsEither(18, 22, 82, 83, 84, 85, 160, 133, 134, 135, 136, 298, 137, 299, 264, 266, 138, 140, 141))
                return VideoStreamType.MP4;
            if (itag.IsEither(43, 100, 278, 242, 243, 244, 247, 248, 271, 313, 272, 302, 303, 308, 315, 330, 331, 332,
                333, 334, 335, 336, 337, 171, 249, 250, 251))
                return VideoStreamType.WebM;
            if (itag.IsEither(13, 17, 36))
                return VideoStreamType.TGPP;
            if (itag.IsEither(5, 6, 34, 35))
                return VideoStreamType.FLV;
            if (itag.IsEither(91, 92, 93, 94, 95, 96, 127, 128))
                return VideoStreamType.TS;

            return VideoStreamType.Unknown;
        }

        private static VideoStreamQuality GetQuality(int itag)
        {
            if (itag.IsEither(17, 91, 160, 219, 278, 330))
                return VideoStreamQuality.Low144;
            if (itag.IsEither(5, 36, 83, 92, 132, 133, 242, 331))
                return VideoStreamQuality.Low240;
            if (itag.IsEither(18, 34, 43, 82, 93, 100, 134, 167, 243, 332))
                return VideoStreamQuality.Medium360;
            if (itag.IsEither(35, 44, 83, 101, 94, 135, 168, 218, 244, 245, 246))
                return VideoStreamQuality.Medium480;
            if (itag.IsEither(22, 45, 84, 102, 95, 136, 169, 244, 247, 298, 302, 334))
                return VideoStreamQuality.High720;
            if (itag.IsEither(37, 46, 85, 96, 137, 299, 170, 248, 303, 335))
                return VideoStreamQuality.High1080;
            if (itag.IsEither(264, 271, 308, 336))
                return VideoStreamQuality.High1440;
            if (itag.IsEither(138, 266, 272, 313, 315, 337))
                return VideoStreamQuality.High2160;
            if (itag.IsEither(38))
                return VideoStreamQuality.High3072;

            return VideoStreamQuality.Unknown;
        }

        private static string GetExtension(VideoStreamType type)
        {
            if (type == VideoStreamType.MP4)
                return "mp4";
            if (type == VideoStreamType.WebM)
                return "webm";
            if (type == VideoStreamType.TGPP)
                return "3gpp";
            if (type == VideoStreamType.FLV)
                return "flv";
            if (type == VideoStreamType.TS)
                return "ts";

            // Default is mp4
            return "mp4";
        }
    }
}