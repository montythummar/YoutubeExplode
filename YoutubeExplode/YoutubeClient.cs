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

        private PlayerSource GetPlayerSource(string version)
        {
            if (version.IsBlank())
                throw new ArgumentNullException(nameof(version));

            // Try get cached player source
            var playerSource = _playerSourceCache.GetOrDefault(version);

            // If not available - decompile a new one
            if (playerSource == null)
            {
                // Get the javascript source URL
                string response =
                    RequestService.GetString($"https://www.youtube.com/yts/jsbin/player-{version}/base.js");
                if (response.IsBlank())
                    throw new Exception("Could not get the video player source code");

                // Decompile
                playerSource = Parser.ParsePlayerSourceJs(response);

                // Cache
                _playerSourceCache[version] = playerSource;
            }

            return playerSource;
        }

        private async Task<PlayerSource> GetPlayerSourceAsync(string version)
        {
            if (version.IsBlank())
                throw new ArgumentNullException(nameof(version));

            // Try get cached player source
            var playerSource = _playerSourceCache.GetOrDefault(version);

            // If not available - decompile a new one
            if (playerSource == null)
            {
                // Get the javascript source URL
                string response =
                    await RequestService.GetStringAsync($"https://www.youtube.com/yts/jsbin/player-{version}/base.js");
                if (response.IsBlank())
                    throw new Exception("Could not get the video player source code");

                // Decompile
                playerSource = Parser.ParsePlayerSourceJs(response);

                // Cache
                _playerSourceCache[version] = playerSource;
            }

            return playerSource;
        }

        private void UnscrambleVideoInfo(PlayerSource playerSource, VideoInfo videoInfo)
        {
            if (playerSource == null)
                throw new ArgumentNullException(nameof(playerSource));
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (!videoInfo.NeedsDeciphering)
                throw new Exception("Given video info does not need to be deciphered");

            // Decipher streams
            foreach (var streamInfo in videoInfo.Streams.Where(s => s.NeedsDeciphering))
            {
                string sig = streamInfo.Signature;
                string newSig = playerSource.Unscramble(sig);
                streamInfo.Url = streamInfo.Url.SetQueryStringParameter("signature", newSig);
                streamInfo.NeedsDeciphering = false;
            }
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
        /// If set to false, you can use <see cref="GetFileSize"/> to get file size of individual streams.
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
            string eurl = $"https://youtube.googleapis.com/v/{videoId}".UrlEncode();
            string response = RequestService.GetString($"https://www.youtube.com/get_video_info?video_id={videoId}&sts=17221&eurl={eurl}");
            if (response.IsBlank())
                throw new Exception("Could not get video info");

            // Parse video info
            var result = Parser.ParseVideoInfoUrlEncoded(response);
            if (result == null)
                throw new Exception("Could not parse video info");

            // Get player version
            response = RequestService.GetString($"https://www.youtube.com/watch?v={videoId}&gl=US&hl=en&has_verified=1&bpctr=9999999999");
            if (response.IsBlank())
                throw new Exception("Could not get player version");

            // Parse player version
            result.PlayerVersion = Parser.ParsePlayerVersionHtml(response);

            // Get additional streams from dash if available
            if (result.DashMpdUrl.IsNotBlank())
            {
                // Get
                response = RequestService.GetString(result.DashMpdUrl);

                // Parse
                if (response.IsNotBlank())
                {
                    var dashStreams = Parser.ParseVideoStreamInfosMpd(response);
                    result.Streams = result.Streams.With(dashStreams).ToArray();
                }
            }

            // Finalize the stream list
            result.Streams = result.Streams
                .OrderByDescending(s => (int) s.Quality)
                .ThenByDescending(s => s.Fps)
                .ToArray();

            // Decipher
            if (result.NeedsDeciphering && decipherIfNeeded)
            {
                DecipherStreams(result);
            }

            // Get file size of streams
            if (getFileSizes)
            {
                foreach (var streamInfo in result.Streams)
                    GetFileSize(streamInfo);
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
            string eurl = $"https://youtube.googleapis.com/v/{videoId}".UrlEncode();
            string response = await RequestService.GetStringAsync($"https://www.youtube.com/get_video_info?video_id={videoId}&sts=17221&eurl={eurl}");
            if (response.IsBlank())
                throw new Exception("Could not get video info");

            // Parse video info
            var result = Parser.ParseVideoInfoUrlEncoded(response);
            if (result == null)
                throw new Exception("Could not parse video info");

            // Get player version
            response = await RequestService.GetStringAsync($"https://www.youtube.com/watch?v={videoId}&gl=US&hl=en&has_verified=1&bpctr=9999999999");
            if (response.IsBlank())
                throw new Exception("Could not get player version");

            // Parse player version
            result.PlayerVersion = Parser.ParsePlayerVersionHtml(response);

            // Get additional streams from dash if available
            if (result.DashMpdUrl.IsNotBlank())
            {
                // Get
                response = await RequestService.GetStringAsync(result.DashMpdUrl);

                // Parse
                if (response.IsNotBlank())
                {
                    var dashStreams = Parser.ParseVideoStreamInfosMpd(response);
                    result.Streams = result.Streams.With(dashStreams).ToArray();
                }
            }

            // Finalize the stream list
            result.Streams = result.Streams
                .OrderByDescending(s => (int) s.Quality)
                .ThenByDescending(s => s.Fps)
                .ToArray();

            // Decipher
            if (result.NeedsDeciphering && decipherIfNeeded)
            {
                await DecipherStreamsAsync(result);
            }

            // Get file size of streams
            if (getFileSizes)
            {
                foreach (var streamInfo in result.Streams)
                    await GetFileSizeAsync(streamInfo);
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

            // Get player source
            var playerSource = GetPlayerSource(videoInfo.PlayerVersion);

            // Unscramble
            UnscrambleVideoInfo(playerSource, videoInfo);
        }

        /// <inheritdoc cref="DecipherStreams"/>
        public async Task DecipherStreamsAsync(VideoInfo videoInfo)
        {
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (!videoInfo.NeedsDeciphering)
                throw new Exception("Given video info does not need to be deciphered");

            // Get player source
            var playerSource = await GetPlayerSourceAsync(videoInfo.PlayerVersion);

            // Unscramble
            UnscrambleVideoInfo(playerSource, videoInfo);
        }

        /// <summary>
        /// Gets and populates the total file size of the video, streamed on the given endpoint
        /// <returns>The file size of the video (in bytes)</returns>
        /// </summary>
        public long GetFileSize(VideoStreamInfo streamInfo)
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

            return streamInfo.FileSize = headers["Content-Length"].ParseLongOrDefault();
        }

        /// <inheritdoc cref="GetFileSize"/>
        public async Task<long> GetFileSizeAsync(VideoStreamInfo streamInfo)
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

            return streamInfo.FileSize = headers["Content-Length"].ParseLongOrDefault();
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