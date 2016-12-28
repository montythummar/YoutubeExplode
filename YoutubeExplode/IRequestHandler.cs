// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <IRequestHandler.cs>
//  Created By: Alexey Golub
//  Date: 26/12/2016
// ------------------------------------------------------------------ 

using System.Collections.Generic;
using System.IO;

namespace YoutubeExplode
{
    /// <summary>
    /// Implemented by classes that handle HTTP requests for YoutubeClient
    /// </summary>
    public interface IRequestHandler
    {
        /// <summary>
        /// Performs a GET request and returns the HTML content of the page as a string
        /// </summary>
        /// <returns>Page HTML content or null if the operation failed</returns>
        string GetHtml(string url);

        /// <summary>
        /// Performs a HEAD request and returns header dictionary
        /// </summary>
        /// <returns>Header dictionary or null if the operation failed</returns>
        IDictionary<string, string> GetHeaders(string url);

        /// <summary>
        /// Performs a GET request and returns the binary content of the response as a stream
        /// </summary>
        /// <returns>Response stream or null if the operation failed</returns>
        Stream DownloadFile(string url);
    }
}
