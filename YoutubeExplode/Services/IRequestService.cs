using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace YoutubeExplode.Services
{
    /// <summary>
    /// Implemented by classes that handle HTTP requests for YoutubeClient
    /// </summary>
    public interface IRequestService
    {
        /// <summary>
        /// Performs a GET request and returns the response content as a string
        /// </summary>
        /// <returns>Respose content as a string or null if the operation failed</returns>
        string GetString(string url);

        /// <summary>
        /// Performs a GET request and returns the response content as a string
        /// </summary>
        /// <returns>Respose content as a string or null if the operation failed</returns>
        Task<string> GetStringAsync(string url);

        /// <summary>
        /// Performs a HEAD request and returns header dictionary
        /// </summary>
        /// <returns>Header dictionary or null if the operation failed</returns>
        IDictionary<string, string> GetHeaders(string url);

        /// <summary>
        /// Performs a HEAD request and returns header dictionary
        /// </summary>
        /// <returns>Header dictionary or null if the operation failed</returns>
        Task<IDictionary<string, string>> GetHeadersAsync(string url);

        /// <summary>
        /// Performs a GET request and returns the binary content of the response as a stream
        /// </summary>
        /// <returns>Response stream or null if the operation failed</returns>
        Stream DownloadFile(string url);

        /// <summary>
        /// Performs a GET request and returns the binary content of the response as a stream
        /// </summary>
        /// <returns>Response stream or null if the operation failed</returns>
        Task<Stream> DownloadFileAsync(string url);
    }
}
