using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode.Internal;

namespace YoutubeExplode.Services
{
    /// <summary>
    /// Uses <see cref="HttpClient"/> for handling requests
    /// </summary>
    public partial class DefaultRequestService : IRequestService, IDisposable
    {
        /// <summary>
        /// Http client in use by the class
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="DefaultRequestService"/>
        /// </summary>
        public DefaultRequestService()
        {
            var httpClientHandler = new HttpClientHandler();
            if (httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpClientHandler.UseCookies = false;
            HttpClient = new HttpClient(httpClientHandler);
        }

        /// <summary>
        /// Creates an instance of <see cref="DefaultRequestService"/> with a custom <see cref="HttpClient"/>
        /// </summary>
        public DefaultRequestService(HttpClient client)
        {
            HttpClient = client;
        }

        /// <inheritdoc />
        public virtual async Task<string> GetStringAsync(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                return await HttpClient.GetStringAsync(url);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public virtual async Task<IDictionary<string, string>> GetHeadersAsync(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, url);
                var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                return NormalizeResponseHeaders(response);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public virtual async Task<Stream> GetStreamAsync(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                return await HttpClient.GetStreamAsync(url);
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            HttpClient.Dispose();
        }
    }

    public partial class DefaultRequestService
    {
        private static IDictionary<string, string> NormalizeResponseHeaders(HttpResponseMessage response)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in response.Headers)
                result.Add(header.Key, header.Value.JoinToString(" "));
            foreach (var header in response.Content.Headers)
                result.Add(header.Key, header.Value.JoinToString(" "));
            return result;
        }
    }
}
