using System;
using System.IO;
using Tyrrrz.Extensions;

namespace YoutubeExplode.DemoConsole
{
    public static class Program
    {
        /// <summary>
        /// If given a youtube url, parses video id from it.
        /// Otherwise returns the same string.
        /// </summary>
        private static string NormalizeId(string input)
        {
            string id;
            if (!YoutubeClient.TryParseVideoId(input, out id))
                id = input;
            return id;
        }

        /// <summary>
        /// Turns file size in bytes into human-readable string
        /// </summary>
        private static string NormalizeFileSize(ulong fileSize)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            decimal size = fileSize;
            var unit = 0;

            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }

            return $"{size:0.#} {units[unit]}";
        }

        public static void Main(string[] args)
        {
            Console.Title = "YoutubeExplode Demo";

            // Get the video ID
            Console.WriteLine("Enter Youtube video ID or URL:");
            string id = Console.ReadLine();
            id = NormalizeId(id);

            Console.WriteLine("Loading . . .");
            Console.WriteLine();

            // Get the video info
            var videoInfo = YoutubeClient.Instance.GetVideoInfo(id);

            // Output some meta data
            Console.WriteLine($"{videoInfo.Title} | {videoInfo.ViewCount:N0} views | {videoInfo.AverageRating:0.##}* rating");
            Console.WriteLine("Streams:");
            for (int i = 0; i < videoInfo.Streams.Length; i++)
            {
                var streamInfo = videoInfo.Streams[i];
                Console.WriteLine($"\t[{i}] {streamInfo.Type} | {streamInfo.Quality} | {streamInfo.Fps} FPS | {NormalizeFileSize(streamInfo.FileSize)}");
            }

            // Get the stream index to download
            Console.WriteLine();
            Console.WriteLine("Enter corresponding number to download:");
            int streamIndex = Console.ReadLine().ParseInt();
            var selectedStream = videoInfo.Streams[streamIndex];

            Console.WriteLine("Loading . . .");
            Console.WriteLine();

            // Compose file name, based on meta data
            string fileName = $"{videoInfo.Title}.{selectedStream.FileExtension}".Without(Path.GetInvalidFileNameChars());

            // Download video
            YoutubeClient.Instance.DownloadVideo(selectedStream, fileName);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }
}