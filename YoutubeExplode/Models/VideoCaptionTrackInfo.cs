namespace YoutubeExplode.Models
{
    /// <summary>
    /// Youtube video subtitles meta data
    /// </summary>
    public class VideoCaptionTrackInfo
    {
        /// <summary>
        /// URL of the subtitles file
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// Subtitles language
        /// </summary>
        public string Language { get; internal set; }

        internal VideoCaptionTrackInfo() { }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Language}";
        }
    }
}