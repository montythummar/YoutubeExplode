namespace YoutubeExplode.Models
{
    /// <summary>
    /// Defines possible adaptive modes
    /// </summary>
    public enum VideoStreamAdaptiveMode
    {
        /// <summary>
        /// Non-adaptive
        /// </summary>
        None,

        /// <summary>
        /// Only contains video
        /// </summary>
        Video,

        /// <summary>
        /// Only contains audio
        /// </summary>
        Audio
    }
}