// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <YoutubeClient.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.IO;
using System.Text.RegularExpressions;
using YoutubeExplode.Models;
using YoutubeExplode.Services;

namespace YoutubeExplode
{
    /// <summary>
    /// YoutubeClient
    /// </summary>
    public class YoutubeClient
    {
        /// <summary>
        /// Default instance of YoutubeClient
        /// </summary>
        public static YoutubeClient Default { get; } = new YoutubeClient();

        /// <summary>
        /// HTTP request handler
        /// </summary>
        public IRequestService RequestService { get; set; } = DefaultRequestService.Instance;

        /// <summary>
        /// Verifies that the given string is a valid youtube video ID
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool VerifyYoutubeID(string videoID)
        {
            return !Regex.IsMatch(videoID, @"[^0-9a-zA-Z_\-]", RegexOptions.CultureInvariant);
        }

        /// <summary>
        /// Get full information about a video by its ID
        /// </summary>
        /// <param name="videoID">The ID of the video</param>
        /// <param name="decipherIfNeeded">When set to true, videos with encrypted signatures will be automatically deciphered. This requires one extra GET request and some computational time. If set to false, the <see cref="VideoInfo"/> will need to be deciphered manually using <see cref="DecipherStreams"/> method. Non-deciphered <see cref="VideoInfo"/> objects are still fully usable, but it will not be possible to access its <see cref="VideoStreamEndpoint"/> by URL</param>
        /// <param name="getFileSizes">When set to true, it will also attempt to get file sizes of all obtained streams. This requires one extra HEAD request per stream. If set to false, you can use <see cref="GetFileSize"/> to get file size of individual streams or <see cref="GetAllFileSizes"/> for all of them. This parameter requires <paramref name="decipherIfNeeded"/> to be set.</param>
        /// <returns><see cref="VideoInfo"/> object with the information on the given video</returns>
        public VideoInfo GetVideoInfo(string videoID, bool decipherIfNeeded = true, bool getFileSizes = true)
        {
            if (videoID.IsBlank())
                throw new ArgumentNullException(nameof(videoID));
            if (!VerifyYoutubeID(videoID))
                throw new ArgumentException("Is not a valid Youtube video ID", nameof(videoID));

            // Grab watch page html code
            string response = RequestService.GetString($"https://youtube.com/watch?v={videoID}");
            if (response.IsBlank())
                throw new Exception("Could not get video watch page (GET request failed)");

            // Try to get video info
            VideoInfo result;
            var jsonMatch = Regex.Match(response, @"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);
            if (jsonMatch.Success)
            {
                // Get it directly from JSON object in the watch page
                result = VideoInfoParser.ParseVideoInfoJson(jsonMatch.Groups[1].Value);
                if (result == null)
                    throw new Exception("Could not parse video info (JSON)");
            }
            else
            {
                // Get it from URL encoded data from internal api
                response = RequestService.GetString($"https://youtube.com/get_video_info?video_id={videoID}");
                if (response.IsBlank())
                    throw new Exception("Could not get URL-encoded video info (GET request failed)");
                result = VideoInfoParser.ParseVideoInfoUrlEncoded(response);
                if (result == null)
                    throw new Exception("Could not parse video info (URL-encoded)");
            }

            // Decipher
            if (result.NeedsDeciphering && decipherIfNeeded)
            {
                DecipherStreams(result);
            }

            // Get file size of streams
            if (getFileSizes)
            {
                GetAllFileSizes(result);
            }

            return result;
        }

        /// <summary>
        /// Deciphers the streams in the given <see cref="VideoInfo"/>
        /// </summary>
        public void DecipherStreams(VideoInfo videoInfo)
        {
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (!videoInfo.NeedsDeciphering)
                throw new Exception("Given video info does not need to be deciphered");
            if (videoInfo.PlayerVersion.IsBlank())
                throw new Exception("Given video info does not have information about the player version");

            // Get the javascript source URL
            string player = videoInfo.PlayerVersion;
            string response = RequestService.GetString($"https://s.ytimg.com/yts/jsbin/player-{player}/base.js");
            if (response.IsBlank())
                throw new Exception("Could not get the video player source code");

            // Decipher
            new Decipherer(response).UnscrambleSignatures(videoInfo);
        }

        /// <summary>
        /// Gets and populates the total file size of the video, streamed on given endpoint
        /// <returns>The populated file size value (in bytes)</returns>
        /// </summary>
        public ulong GetFileSize(VideoStreamEndpoint stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (stream.Url.IsBlank())
                throw new Exception("Given stream does not have an URL");
            if (stream.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            // Get the headers
            var headers = RequestService.GetHeaders(stream.Url);
            if (headers == null)
                throw new Exception("Could not obtain headers (HEAD request failed)");

            return stream.FileSize = headers.GetOrDefault("Content-Length").ParseUlongOrDefault();
        }

        /// <summary>
        /// Gets and populates the total file sizes of all videos, streamed on endpoints of current video object
        /// </summary>
        public void GetAllFileSizes(VideoInfo videoInfo)
        {
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (videoInfo.Streams == null)
                throw new Exception("There are no streams in the given video info");

            foreach (var stream in videoInfo.Streams)
                GetFileSize(stream);
        }

        /// <summary>
        /// Downloads the given video stream
        /// </summary>
        public Stream DownloadVideo(VideoStreamEndpoint stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (stream.Url.IsBlank())
                throw new Exception("Given stream does not have an URL");
            if (stream.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            return RequestService.DownloadFile(stream.Url);
        }

        /// <summary>
        /// Downloads the given video stream and saves it to a file
        /// </summary>
        public void DownloadVideo(VideoStreamEndpoint stream, string filePath)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (stream.Url.IsBlank())
                throw new Exception("Given stream does not have an URL");
            if (stream.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            using (var input = RequestService.DownloadFile(stream.Url))
            {
                if (input == null)
                    throw new Exception("Could not download the given video stream");

                using (var output = File.Create(filePath))
                    input.CopyTo(output);
            }
        }

        /// <summary>
        /// Parses video ID from a youtube video URL
        /// </summary>
        /// <returns>Video ID</returns>
        public string ParseVideoID(string videoURL)
        {
            var match = Regex.Match(videoURL, @"[?&]v=(.+?)(?:&|$)",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new FormatException("Could not parse Video ID from given string");
            return match.Groups[1].Value;
        }

        /// <summary>
        /// Tries to parse video ID from a youtube video URL
        /// </summary>
        /// <returns>Whether the execution was successful or not</returns>
        public bool TryParseVideoID(string videoURL, out string videoID)
        {
            videoID = default(string);
            var match = Regex.Match(videoURL, @"[?&]v=(.+?)(?:&|$)",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            if (match.Success)
                videoID = match.Groups[1].Value;
            return match.Success;
        }
    }
}