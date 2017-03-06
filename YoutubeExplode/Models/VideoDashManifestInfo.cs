namespace YoutubeExplode.Models
{
    /// <summary>
    /// Dash manifest meta data
    /// </summary>
    public class VideoDashManifestInfo
    {
        /// <summary>
        /// URL for the dash manifest
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// Authorization signature
        /// </summary>
        public string Signature { get; internal set; }

        /// <summary>
        /// Whether the signature needs to be deciphered before manifest can be accessed by URL
        /// </summary>
        public bool NeedsDeciphering { get; internal set; }

        internal VideoDashManifestInfo()
        {
        }
    }
}