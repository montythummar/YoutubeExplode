// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <HttpClient.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace YoutubeExplode
{
    internal class HttpClient
    {
        private const int BufferSize = 1024;

        public string UserAgent { get; set; } =
            // Chrome on Windows10
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36";

        public string Accept { get; set; } =
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";

        public bool AllowRedirect { get; set; } = true;


        /// <summary>
        /// Creates a web request with current properties and given URL
        /// </summary>
        private HttpWebRequest CreateGenericRequest(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.UserAgent = UserAgent;
            request.Accept = Accept;
            request.AllowAutoRedirect = AllowRedirect;
            return request;
        }

        /// <summary>
        /// Performs a web request and returns the output stream of data
        /// </summary>
        private MemoryStream PerformRequest(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse) request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null) return null;

                var outputStream = new MemoryStream();
                var buffer = new byte[BufferSize];
                int bytesRead;
                do
                {
                    bytesRead = responseStream.Read(buffer, 0, BufferSize);
                    outputStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);

                outputStream.Seek(0, SeekOrigin.Begin);
                return outputStream;
            }
        }

        /// <summary>
        /// Download target to stream
        /// </summary>
        public MemoryStream Download(string url)
        {
            var request = CreateGenericRequest(url);
            request.Method = "GET";

            // Get response
            return PerformRequest(request);
        }

        /// <summary>
        /// Get a page by URL and return its body
        /// </summary>
        public string Get(string url)
        {
            var request = CreateGenericRequest(url);
            request.Method = "GET";

            // Get response
            using (var stream = PerformRequest(request))
                return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Posts data to URL and returns response body
        /// </summary>
        public string Post(string url, IDictionary<string, string> parameters)
        {
            var request = CreateGenericRequest(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // Add parameters to request
            var parametersArray = parameters.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}").ToArray();
            string contentBody = string.Join("&", parametersArray);
            var dataBytes = Encoding.UTF8.GetBytes(contentBody);
            request.ContentLength = dataBytes.Length;
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(dataBytes, 0, dataBytes.Length);

            // Get response
            using (var stream = PerformRequest(request))
                return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}