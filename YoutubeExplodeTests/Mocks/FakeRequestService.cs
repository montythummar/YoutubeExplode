using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tyrrrz.Extensions;
using YoutubeExplode.Services;

namespace YoutubeExplode.Tests.Mocks
{
    public class FakeRequestService : IRequestService
    {
        private static byte[] ReadResource(string resourceId)
        {
            var input = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceId);
            if (input == null) return null;

            using (input)
            using (var output = new MemoryStream())
            {
                input.CopyTo(output);
                return output.ToArray();
            }
        }

        private static string ReadResourceAsString(string resourceId)
        {
            return ReadResource(resourceId).GetString();
        }

        public bool IsWatchPageWorking { get; set; } = true;
        public bool IsInternalApiWorking { get; set; } = true;

        public string GetString(string url)
        {
            // Watch page
            if (Regex.IsMatch(url, @"http(?:s)?://(?:www.)?youtube.com/watch\?v=(.+?)"))
            {
                if (IsWatchPageWorking)
                    return ReadResourceAsString("YoutubeExplode.Tests.Mocks.FakeWatchPageResponse.txt");
                return "junk";
            }
            // Internal API
            if (Regex.IsMatch(url, @"http(?:s)?://(?:www.)?youtube.com/get_video_info\?video_id=(.+?)"))
            {
                if (IsInternalApiWorking)
                    return ReadResourceAsString("YoutubeExplode.Tests.Mocks.FakeInternalApiResponse.txt");
                return "junk";
            }
            // Player JS source code
            if (Regex.IsMatch(url, @"http(?:s)?://(?:www.)?youtube.com/yts/jsbin/player-(.+?)/base.js"))
            {
                return ReadResourceAsString("YoutubeExplode.Tests.Mocks.FakePlayerSourceCodeResponse.txt");
            }

            throw new Exception("Unexpected request url");
        }

        public IDictionary<string, string> GetHeaders(string url)
        {
            return new Dictionary<string, string> {{"Content-Length", "1337"}};
        }

        public Stream DownloadFile(string url)
        {
            var fakeData = "fake video file".GetBytes();
            return new MemoryStream(fakeData);
        }

        public async Task<string> GetStringAsync(string url)
        {
            await Task.Yield();
            return GetString(url);
        }

        public async Task<IDictionary<string, string>> GetHeadersAsync(string url)
        {
            await Task.Yield();
            return GetHeaders(url);
        }

        public async Task<Stream> DownloadFileAsync(string url)
        {
            await Task.Yield();
            return DownloadFile(url);
        }
    }
}