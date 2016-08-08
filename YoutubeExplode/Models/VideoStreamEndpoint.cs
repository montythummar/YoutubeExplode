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
        /// <summary>
        /// URL of the stream
        /// </summary>
        public string URL { get; internal set; }

        /// <summary>
        /// Type of the video stream
        /// </summary>
        public string Type { get; internal set; }

        /// <summary>
        /// Quality of the video stream
        /// </summary>
        public string Quality { get; internal set; }

        internal VideoStreamEndpoint() { }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        public override string ToString()
        {
            return $"{Type} | {Quality}";
        }
    }
}