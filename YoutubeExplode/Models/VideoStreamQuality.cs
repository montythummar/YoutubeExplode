// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <VideoStreamQuality.cs>
//  Created By: Alexey Golub
//  Date: 10/08/2016
// ------------------------------------------------------------------ 

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
        /// HD video stream
        /// </summary>
        High,

        /// <summary>
        /// Medium-quality video stream
        /// </summary>
        Medium,

        /// <summary>
        /// Low-quality video stream
        /// </summary>
        Low
    }
}