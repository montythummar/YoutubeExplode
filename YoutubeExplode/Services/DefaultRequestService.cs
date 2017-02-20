// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <DefaultRequestHandler.cs>
//  Created By: Alexey Golub
//  Date: 26/12/2016
// ------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeExplode.Services
{
    /// <summary>
    /// Uses <see cref="WebRequest"/> for handling requests
    /// </summary>
    public partial class DefaultRequestService : IRequestService
    {
        /// <summary>
        /// Default instance
        /// </summary>
        public static DefaultRequestService Instance { get; } = new DefaultRequestService();

        /// <inheritdoc />
        public string GetString(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "GET";

                using (var response = request.GetResponse())
                {
                    var data = GetArray(response.GetResponseStream());
                    return Encoding.UTF8.GetString(data);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetHeaders(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "HEAD";

                using (var response = request.GetResponse())
                {
                    return WebHeadersToDictionary(response.Headers);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public Stream DownloadFile(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "GET";

                return request.GetResponse().GetResponseStream();
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetStringAsync(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "GET";

                using (var response = await request.GetResponseAsync())
                {
                    var data = await GetArrayAsync(response.GetResponseStream());
                    return Encoding.UTF8.GetString(data);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<IDictionary<string, string>> GetHeadersAsync(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "HEAD";

                using (var response = await request.GetResponseAsync())
                {
                    return WebHeadersToDictionary(response.Headers);
                }
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<Stream> DownloadFileAsync(string url)
        {
            if (url.IsBlank())
                throw new ArgumentNullException(nameof(url));

            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = "GET";

                return (await request.GetResponseAsync()).GetResponseStream();
            }
            catch
            {
                return null;
            }
        }
    }

    public partial class DefaultRequestService
    {
        /// <summary>
        /// Reads stream into an array
        /// </summary>
        protected static byte[] GetArray(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            byte[] buffer = new byte[1024];
            using (input)
            using (var ms = new MemoryStream())
            {
                int bytesRead;
                do
                {
                    bytesRead = input.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Reads stream into an array
        /// </summary>
        protected static async Task<byte[]> GetArrayAsync(Stream input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            byte[] buffer = new byte[1024];
            using (input)
            using (var ms = new MemoryStream())
            {
                int bytesRead;
                do
                {
                    bytesRead = await input.ReadAsync(buffer, 0, buffer.Length);
                    await ms.WriteAsync(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts <see cref="WebHeaderCollection"/> to <see cref="IDictionary{TKey,TValue}" />
        /// </summary>
        protected static IDictionary<string, string> WebHeadersToDictionary(WebHeaderCollection headers)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            for (int i = 0; i < headers.Count; i++)
            {
                string headerName = headers.GetKey(i);
                string headerValue = headers.Get(i);
                result.Add(headerName, headerValue);
            }
            return result;
        }
    }
}
