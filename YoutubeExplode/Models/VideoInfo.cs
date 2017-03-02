using System;

namespace YoutubeExplode.Models
{
    /// <summary>
    /// Youtube video meta data
    /// </summary>
    public class VideoInfo
    {
        /// <summary>
        /// Video ID
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// Video title
        /// </summary>
        public string Title { get; internal set; }

        /// <summary>
        /// Video author
        /// </summary>
        public string Author { get; internal set; }

        /// <summary>
        /// Length of the video
        /// </summary>
        public TimeSpan Length { get; internal set; }

        /// <summary>
        /// View count
        /// </summary>
        public long ViewCount { get; internal set; }

        /// <summary>
        /// Average user rating.
        /// Ranges from 0 stars to 5 stars.
        /// </summary>
        public double AverageRating { get; internal set; }

        /// <summary>
        /// Normalized average user rating.
        /// Ranges from 0 to 1.
        /// Also represents the "thumbs up" ratio in the new rating system.
        /// </summary>
        public double NormalizedAverageRating => AverageRating / 5;

        /// <summary>
        /// Keywords used for searching
        /// </summary>
        public string[] Keywords { get; set; }

        /// <summary>
        /// URL for the thumbnail image
        /// </summary>
        public string ImageThumbnail => $"https://img.youtube.com/vi/{Id}/maxresdefault.jpg";

        /// <summary>
        /// URL for the default resolution image (not always available)
        /// </summary>
        public string ImageStandardRes => $"https://img.youtube.com/vi/{Id}/sddefault.jpg";

        /// <summary>
        /// URL for the highest resolution image (not always available)
        /// </summary>
        public string ImageMaxRes => $"https://img.youtube.com/vi/{Id}/maxresdefault.jpg";

        /// <summary>
        /// URL for the high resolution image
        /// </summary>
        public string ImageHighRes => $"https://img.youtube.com/vi/{Id}/hqdefault.jpg";

        /// <summary>
        /// URL for the medium resolution image
        /// </summary>
        public string ImageMediumRes => $"https://img.youtube.com/vi/{Id}/mqdefault.jpg";

        /// <summary>
        /// Collection of watermark URLs
        /// </summary>
        public string[] Watermarks { get; internal set; }

        /// <summary>
        /// Whether this video is listed publicly
        /// </summary>
        public bool IsListed { get; internal set; }

        /// <summary>
        /// Whether it is allowed to leave user rating on this video
        /// </summary>
        public bool IsRatingAllowed { get; internal set; }

        /// <summary>
        /// Whether the audio has been muted on this video
        /// </summary>
        public bool IsMuted { get; internal set; }

        /// <summary>
        /// Whether it is allowed to embed this video outside of youtube
        /// </summary>
        public bool IsEmbeddingAllowed { get; internal set; }

        /// <summary>
        /// Whether this video has closed captions
        /// </summary>
        public bool HasClosedCaptions => CaptionTracks != null && CaptionTracks.Length > 0;

        /// <summary>
        /// Video streams
        /// </summary>
        public VideoStreamInfo[] Streams { get; internal set; }

        /// <summary>
        /// Closed captions
        /// </summary>
        public VideoCaptionTrackInfo[] CaptionTracks { get; internal set; }

        /// <summary>
        /// Whether this video uses an encrypted signature for its streams that needs to be deciphered before the streams can be accessed
        /// </summary>
        public bool NeedsDeciphering { get; internal set; }

        /// <summary>
        /// Version of the video player, used for this video's playback
        /// </summary>
        internal string PlayerVersion { get; set; }

        /// <summary>
        /// Url of dash xml
        /// </summary>
        internal string DashMpdUrl { get; set; }

        internal VideoInfo() { }
    }
}