using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeExplode.Internal;
using YoutubeExplode.Models;
using YoutubeExplode.Services;

namespace YoutubeExplode
{
    /// <summary>
    /// YoutubeClient
    /// </summary>
    public partial class YoutubeClient
    {
        private readonly Dictionary<string, PlayerSource> _playerSourceCache = new Dictionary<string, PlayerSource>();

        /// <summary>
        /// HTTP request handler
        /// </summary>
        public IRequestService RequestService { get; set; }

        /// <summary>
        /// Construct with custom services
        /// </summary>
        public YoutubeClient(IRequestService requestService)
        {
            RequestService = requestService;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public YoutubeClient()
            : this(DefaultRequestService.Instance)
        {
        }

        /// <summary>
        /// Tries to get video info from the watch page
        /// </summary>
        /// <returns>Result if successful, null if not</returns>
        protected VideoInfo GetVideoInfoFromWatchPage(string videoId)
        {
            if (videoId.IsBlank())
                throw new ArgumentNullException(nameof(videoId));
            if (!ValidateVideoId(videoId))
                throw new ArgumentException("Is not a valid Youtube video ID", nameof(videoId));

            // Get
            string response = RequestService.GetString($"https://youtube.com/watch?v={videoId}");
            if (response.IsBlank())
                throw new Exception("Could not get video watch page (GET request failed)");

            // Parse
            var jsonMatch = Regex.Match(response, @"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);
            return jsonMatch.Success ? Parser.ParseVideoInfoJson(jsonMatch.Groups[1].Value) : null;
        }

        /// <inheritdoc cref="GetVideoInfoFromWatchPage"/>
        protected async Task<VideoInfo> GetVideoInfoFromWatchPageAsync(string videoId)
        {
            if (videoId.IsBlank())
                throw new ArgumentNullException(nameof(videoId));
            if (!ValidateVideoId(videoId))
                throw new ArgumentException("Is not a valid Youtube video ID", nameof(videoId));

            // Get
            string response = await RequestService.GetStringAsync($"https://youtube.com/watch?v={videoId}");
            if (response.IsBlank())
                throw new Exception("Could not get video watch page (GET request failed)");

            // Parse
            var jsonMatch = Regex.Match(response, @"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);
            return jsonMatch.Success ? Parser.ParseVideoInfoJson(jsonMatch.Groups[1].Value) : null;
        }

        /// <summary>
        /// Tries to get video info from the internal api
        /// </summary>
        /// <returns>Result if successful, null if not</returns>
        protected VideoInfo GetVideoInfoFromInternalApi(string videoId)
        {
            if (videoId.IsBlank())
                throw new ArgumentNullException(nameof(videoId));
            if (!ValidateVideoId(videoId))
                throw new ArgumentException("Is not a valid Youtube video ID", nameof(videoId));

            // Get
            string response = RequestService.GetString($"https://youtube.com/get_video_info?video_id={videoId}");
            if (response.IsBlank())
                throw new Exception("Could not get URL-encoded video info (GET request failed)");

            // Parse
            return Parser.ParseVideoInfoUrlEncoded(response);
        }

        /// <inheritdoc cref="GetVideoInfoFromInternalApi"/>
        protected async Task<VideoInfo> GetVideoInfoFromInternalApiAsync(string videoId)
        {
            if (videoId.IsBlank())
                throw new ArgumentNullException(nameof(videoId));
            if (!ValidateVideoId(videoId))
                throw new ArgumentException("Is not a valid Youtube video ID", nameof(videoId));

            // Get
            string response = await RequestService.GetStringAsync($"https://youtube.com/get_video_info?video_id={videoId}");
            if (response.IsBlank())
                throw new Exception("Could not get URL-encoded video info (GET request failed)");

            // Parse
            return Parser.ParseVideoInfoUrlEncoded(response);
        }

        /// <summary>
        /// Get full information about a video by its ID
        /// </summary>
        /// 
        /// <param name="videoId">The ID of the video</param>
        /// 
        /// <param name="decipherIfNeeded">
        /// When set to true, videos with encrypted signatures will be automatically deciphered.
        /// This requires one extra GET request and some computational time.
        /// If set to false, the <see cref="VideoInfo"/> will need to be deciphered manually using <see cref="DecipherStreams"/> method.
        /// Non-deciphered <see cref="VideoInfo"/> objects are still fully usable, but it will not be possible to access its <see cref="VideoStreamInfo"/> by URL
        /// </param>
        /// 
        /// <param name="getFileSizes">
        /// When set to true, it will also attempt to get file sizes of all obtained streams.
        /// This requires one extra HEAD request per stream.
        /// If set to false, you can use <see cref="GetFileSize"/> to get file size of individual streams or <see cref="GetAllFileSizes"/> for all of them.
        /// This parameter requires <paramref name="decipherIfNeeded"/> to be set.
        /// </param>
        /// 
        /// <returns><see cref="VideoInfo"/> object with the information on the given video</returns>
        public VideoInfo GetVideoInfo(string videoId, bool decipherIfNeeded = true, bool getFileSizes = true)
        {
            if (videoId.IsBlank())
                throw new ArgumentNullException(nameof(videoId));
            if (!ValidateVideoId(videoId))
                throw new ArgumentException("Is not a valid Youtube video ID", nameof(videoId));
            if (getFileSizes && !decipherIfNeeded)
                throw new ArgumentException($"{nameof(getFileSizes)} flag can only be set along with {nameof(decipherIfNeeded)}");

            // Get video info
            var result = GetVideoInfoFromWatchPage(videoId) ?? GetVideoInfoFromInternalApi(videoId);
            if (result == null)
                throw new Exception("Could not obtain video info from either sources");

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

        /// <inheritdoc cref="GetVideoInfo"/>
        public async Task<VideoInfo> GetVideoInfoAsync(string videoId, bool decipherIfNeeded = true, bool getFileSizes = true)
        {
            if (videoId.IsBlank())
                throw new ArgumentNullException(nameof(videoId));
            if (!ValidateVideoId(videoId))
                throw new ArgumentException("Is not a valid Youtube video ID", nameof(videoId));
            if (getFileSizes && !decipherIfNeeded)
                throw new ArgumentException($"{nameof(getFileSizes)} flag can only be set along with {nameof(decipherIfNeeded)}");

            // Get video info
            var result = await GetVideoInfoFromWatchPageAsync(videoId) ?? await GetVideoInfoFromInternalApiAsync(videoId);
            if (result == null)
                throw new Exception("Could not obtain video info from either sources");

            // Decipher
            if (result.NeedsDeciphering && decipherIfNeeded)
            {
                await DecipherStreamsAsync(result);
            }

            // Get file size of streams
            if (getFileSizes)
            {
                await GetAllFileSizesAsync(result);
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

            // Check if anything needs deciphering
            if (videoInfo.Streams == null || !videoInfo.Streams.Any())
            {
                videoInfo.NeedsDeciphering = false;
                return;
            }

            // Try get cached player source
            var playerSource = _playerSourceCache.GetOrDefault(videoInfo.PlayerVersion);

            // If not available - decompile a new one
            if (playerSource == null)
            {
                // Get the javascript source URL
                string response = RequestService.GetString($"https://youtube.com/yts/jsbin/player-{videoInfo.PlayerVersion}/base.js");
                if (response.IsBlank())
                    throw new Exception("Could not get the video player source code");

                // Decompile
                playerSource = Parser.ParsePlayerSourceJs(response);

                // Cache
                _playerSourceCache[videoInfo.PlayerVersion] = playerSource;
            }

            // Decipher
            foreach (var streamInfo in videoInfo.Streams.Where(s => s.NeedsDeciphering))
            {
                string newSig = playerSource.Unscramble(streamInfo.Signature);
                streamInfo.Url = streamInfo.Url.SetQueryStringParameter("signature", newSig);
                streamInfo.NeedsDeciphering = false;
            }
            videoInfo.NeedsDeciphering = false;
        }

        /// <inheritdoc cref="DecipherStreams"/>
        public async Task DecipherStreamsAsync(VideoInfo videoInfo)
        {
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (!videoInfo.NeedsDeciphering)
                throw new Exception("Given video info does not need to be deciphered");
            if (videoInfo.PlayerVersion.IsBlank())
                throw new Exception("Given video info does not have information about the player version");

            // Check if anything needs deciphering
            if (videoInfo.Streams == null || !videoInfo.Streams.Any())
            {
                videoInfo.NeedsDeciphering = false;
                return;
            }

            // Try get cached player source
            var playerSource = _playerSourceCache.GetOrDefault(videoInfo.PlayerVersion);

            // If not available - decompile a new one
            if (playerSource == null)
            {
                // Get the javascript source URL
                string response = await RequestService.GetStringAsync($"https://youtube.com/yts/jsbin/player-{videoInfo.PlayerVersion}/base.js");
                if (response.IsBlank())
                    throw new Exception("Could not get the video player source code");

                // Decompile
                playerSource = Parser.ParsePlayerSourceJs(response);

                // Cache
                _playerSourceCache[videoInfo.PlayerVersion] = playerSource;
            }

            // Decipher
            foreach (var streamInfo in videoInfo.Streams.Where(s => s.NeedsDeciphering))
            {
                string newSig = playerSource.Unscramble(streamInfo.Signature);
                streamInfo.Url = streamInfo.Url.SetQueryStringParameter("signature", newSig);
                streamInfo.NeedsDeciphering = false;
            }
            videoInfo.NeedsDeciphering = false;
        }

        /// <summary>
        /// Gets and populates the total file size of the video, streamed on the given endpoint
        /// <returns>The file size of the video (in bytes)</returns>
        /// </summary>
        public ulong GetFileSize(VideoStreamInfo streamInfo)
        {
            if (streamInfo == null)
                throw new ArgumentNullException(nameof(streamInfo));
            if (streamInfo.Url.IsBlank())
                throw new Exception("Given stream does not have a URL");
            if (streamInfo.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            // Get the headers
            var headers = RequestService.GetHeaders(streamInfo.Url);
            if (headers == null)
                throw new Exception("Could not obtain headers (HEAD request failed)");

            // Get file size header
            if (!headers.ContainsKey("Content-Length"))
                throw new Exception("Content-Length header not found");

            return streamInfo.FileSize = headers["Content-Length"].ParseUlongOrDefault();
        }

        /// <inheritdoc cref="GetFileSize"/>
        public async Task<ulong> GetFileSizeAsync(VideoStreamInfo streamInfo)
        {
            if (streamInfo == null)
                throw new ArgumentNullException(nameof(streamInfo));
            if (streamInfo.Url.IsBlank())
                throw new Exception("Given stream does not have a URL");
            if (streamInfo.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            // Get the headers
            var headers = await RequestService.GetHeadersAsync(streamInfo.Url);
            if (headers == null)
                throw new Exception("Could not obtain headers (HEAD request failed)");

            // Get file size header
            if (!headers.ContainsKey("Content-Length"))
                throw new Exception("Content-Length header not found");

            return streamInfo.FileSize = headers["Content-Length"].ParseUlongOrDefault();
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

            // Get file sizes for all streams
            foreach (var streamInfo in videoInfo.Streams)
                GetFileSize(streamInfo);
        }

        /// <inheritdoc cref="GetAllFileSizes"/>
        public async Task GetAllFileSizesAsync(VideoInfo videoInfo)
        {
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (videoInfo.Streams == null)
                throw new Exception("There are no streams in the given video info");

            // Get file sizes for all streams
            foreach (var streamInfo in videoInfo.Streams)
                await GetFileSizeAsync(streamInfo);
        }

        /// <summary>
        /// Downloads the given video stream
        /// </summary>
        public Stream DownloadVideo(VideoStreamInfo streamInfo)
        {
            if (streamInfo == null)
                throw new ArgumentNullException(nameof(streamInfo));
            if (streamInfo.Url.IsBlank())
                throw new Exception("Given stream does not have a URL");
            if (streamInfo.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            // Get stream
            var stream = RequestService.DownloadFile(streamInfo.Url);
            if (stream == null)
                throw new Exception("Could not get response stream");

            return stream;
        }

        /// <inheritdoc cref="DownloadVideo(VideoStreamInfo)"/>
        public async Task<Stream> DownloadVideoAsync(VideoStreamInfo streamInfo)
        {
            if (streamInfo == null)
                throw new ArgumentNullException(nameof(streamInfo));
            if (streamInfo.Url.IsBlank())
                throw new Exception("Given stream does not have a URL");
            if (streamInfo.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            // Get stream
            var stream = await RequestService.DownloadFileAsync(streamInfo.Url);
            if (stream == null)
                throw new Exception("Could not get response stream");

            return stream;
        }

        /// <summary>
        /// Downloads the given video stream and saves it to a file
        /// </summary>
        public void DownloadVideo(VideoStreamInfo streamInfo, string filePath)
        {
            if (streamInfo == null)
                throw new ArgumentNullException(nameof(streamInfo));
            if (streamInfo.Url.IsBlank())
                throw new Exception("Given stream does not have a URL");
            if (streamInfo.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            // Get the stream
            var input = RequestService.DownloadFile(streamInfo.Url);
            if (input == null)
                throw new Exception("Could not get response stream");

            // Write to file
            using (input)
            using (var output = File.Create(filePath))
                input.CopyTo(output);
        }

        /// <inheritdoc cref="DownloadVideo(VideoStreamInfo,string)"/>
        public async Task DownloadVideoAsync(VideoStreamInfo streamInfo, string filePath)
        {
            if (streamInfo == null)
                throw new ArgumentNullException(nameof(streamInfo));
            if (streamInfo.Url.IsBlank())
                throw new Exception("Given stream does not have a URL");
            if (streamInfo.NeedsDeciphering)
                throw new Exception("Given stream's signature needs to be deciphered first");

            // Get the stream
            var input = await RequestService.DownloadFileAsync(streamInfo.Url);
            if (input == null)
                throw new Exception("Could not get response stream");

            // Write to file
            using (input)
            using (var output = File.Create(filePath))
                await input.CopyToAsync(output);
        }
    }

    public partial class YoutubeClient
    {
        /// <summary>
        /// Default instance of YoutubeClient
        /// </summary>
        public static YoutubeClient Instance { get; } = new YoutubeClient();

        /// <summary>
        /// Verifies that the given string is a valid youtube video ID
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateVideoId(string videoId)
        {
            if (videoId.IsBlank())
                throw new ArgumentNullException(nameof(videoId));

            return !Regex.IsMatch(videoId, @"[^0-9a-zA-Z_\-]");
        }

        /// <summary>
        /// Parses video ID from a youtube video URL
        /// </summary>
        /// <returns>Video ID</returns>
        public static string ParseVideoId(string videoUrl)
        {
            if (videoUrl.IsBlank())
                throw new ArgumentNullException(nameof(videoUrl));

            var match = Regex.Match(videoUrl, @"[?&]v=(.+?)(?:&|$)");
            if (!match.Success)
                throw new FormatException("Could not parse Video ID from given string");
            return match.Groups[1].Value;
        }

        /// <summary>
        /// Tries to parse video ID from a youtube video URL
        /// </summary>
        /// <returns>Whether the execution was successful or not</returns>
        public static bool TryParseVideoId(string videoUrl, out string videoId)
        {
            if (videoUrl.IsBlank())
                throw new ArgumentNullException(nameof(videoUrl));

            videoId = default(string);
            var match = Regex.Match(videoUrl, @"[?&]v=(.+?)(?:&|$)");
            if (match.Success)
                videoId = match.Groups[1].Value;
            return match.Success;
        }
    }
}