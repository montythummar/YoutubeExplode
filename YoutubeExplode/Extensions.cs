using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode.Internal;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    /// <summary>
    /// Extensions for YoutubeExplode
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Downloads video to file
        /// </summary>
        public static void DownloadVideo(this YoutubeClient client, VideoStreamInfo streamInfo, string filePath)
        {
            if (filePath.IsBlank())
                throw new ArgumentNullException(nameof(filePath));

            // Get stream
            var stream = client.DownloadVideo(streamInfo);

            // Output to fiile
            using (stream)
            using (var fileStream = File.Create(filePath))
                stream.CopyTo(fileStream);
        }

        /// <summary>
        /// Downloads video to file
        /// </summary>
        public static async Task DownloadVideoAsync(this YoutubeClient client, VideoStreamInfo streamInfo, string filePath)
        {
            if (filePath.IsBlank())
                throw new ArgumentNullException(nameof(filePath));

            // Get stream
            var stream = await client.DownloadVideoAsync(streamInfo);

            // Output to fiile
            using (stream)
            using (var fileStream = File.Create(filePath))
                await stream.CopyToAsync(fileStream);
        }
    }
}