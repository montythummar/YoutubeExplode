// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <DefaultRequestHandler.cs>
//  Created By: Alexey Golub
//  Date: 26/12/2016
// ------------------------------------------------------------------ 

using System.Collections.Generic;
using System.Net;
using System.Text;

namespace YoutubeExplode
{
    /// <summary>
    /// Uses <see cref="WebRequest"/> for handling requests
    /// </summary>
    public sealed class DefaultRequestHandler : IRequestHandler
    {
        /// <summary>
        /// Default instance
        /// </summary>
        public static DefaultRequestHandler Default { get; } = new DefaultRequestHandler();

        /// <inheritdoc />
        public string GetHtml(string url)
        {
            try
            {
                var req = WebRequest.CreateHttp(url);
                req.Method = "GET";
                using (var response = req.GetResponse())
                    return Encoding.UTF8.GetString(response.GetResponseStream().ToArray());
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetHeaders(string url)
        {
            try
            {
                var req = WebRequest.CreateHttp(url);
                req.Method = "HEAD";

                var result = new Dictionary<string, string>();
                using (var response = req.GetResponse())
                {
                    for (int i = 0; i < response.Headers.Count; i++)
                    {
                        string headerName = response.Headers.GetKey(i);
                        string headerValue = response.Headers[headerName];
                        result.Add(headerName, headerValue);
                    }
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
