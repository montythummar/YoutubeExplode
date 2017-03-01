using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Models;

namespace YoutubeExplode.Internal
{
    internal static class Parser
    {
        private static IEnumerable<VideoStreamInfo> ParseVideoStreamInfosUrlEncoded(string rawUrlEncoded)
        {
            if (rawUrlEncoded.IsBlank())
                throw new ArgumentNullException(nameof(rawUrlEncoded));

            foreach (var streamRaw in rawUrlEncoded.Split(","))
            {
                var dic = ParseDictionaryUrlEncoded(streamRaw);

                // Extract values
                string sig = dic.GetOrDefault("s");
                bool needsDeciphering = !sig.IsBlank();
                string url = dic.GetOrDefault("url");
                string type = dic.GetOrDefault("type");
                string quality = dic.GetOrDefault("quality_label") ??
                                 dic.GetOrDefault("quality");
                string resolution = dic.GetOrDefault("size") ?? "0x0";
                ulong bitrate = dic.GetOrDefault("bitrate").ParseUlongOrDefault();
                double fps = dic.GetOrDefault("fps").ParseDoubleOrDefault();

                // Yield a stream object
                yield return new VideoStreamInfo
                {
                    Signature = sig,
                    NeedsDeciphering = needsDeciphering,
                    Url = url,
                    TypeString = type,
                    QualityString = quality,
                    ResolutionString = resolution,
                    Bitrate = bitrate,
                    Fps = fps
                };
            }
        }

        private static Dictionary<string, string> ParseDictionaryUrlEncoded(string rawUrlEncoded)
        {
            if (rawUrlEncoded.IsBlank())
                throw new ArgumentNullException(nameof(rawUrlEncoded));

            var dic = new Dictionary<string, string>();
            var keyValuePairsRaw = rawUrlEncoded.Split("&");
            foreach (string keyValuePairRaw in keyValuePairsRaw)
            {
                string keyValuePairRawDecoded = keyValuePairRaw.UrlDecode();
                if (keyValuePairRawDecoded.IsBlank())
                    continue;

                // Look for the equals sign
                int equalsPos = keyValuePairRawDecoded.IndexOf('=');
                if (equalsPos <= 0)
                    continue;

                // Get the key and value
                string key = keyValuePairRawDecoded.Substring(0, equalsPos);
                string value = equalsPos < keyValuePairRawDecoded.Length
                    ? keyValuePairRawDecoded.Substring(equalsPos + 1)
                    : "";

                // Add to dictionary
                dic[key] = value;
            }

            return dic;
        }

        private static string GetFunctionCallFromLineJs(string rawJs)
        {
            if (rawJs.IsBlank())
                throw new ArgumentNullException(nameof(rawJs));

            var match = Regex.Match(rawJs, @"\w+\.(\w+)\(");
            return match.Groups[1].Value;
        }

        private static IEnumerable<IScramblingOperation> ParseScramblingOperationsJs(string rawJs)
        {
            if (rawJs.IsBlank())
                throw new ArgumentNullException(nameof(rawJs));

            // Get the name of the function that handles deciphering
            var funcNameMatch = Regex.Match(rawJs, @"\""signature"",\s?([a-zA-Z0-9\$]+)\(");
            if (!funcNameMatch.Success)
                throw new Exception("Could not find the entry function for signature deciphering");
            string funcName = funcNameMatch.Groups[1].Value;

            // Escape dollar sign
            funcName = funcName.Replace("$", "\\$");

            // Get the body of the function
            var funcBodyMatch = Regex.Match(rawJs, @"(?!h\.)" + funcName + @"=function\(\w+\)\{.*?\}",
                RegexOptions.Singleline);
            if (!funcBodyMatch.Success)
                throw new Exception("Could not get the signature decipherer function body");
            string funcBody = funcBodyMatch.Value;
            var funcLines = funcBody.Split(";");

            // Identify scrambling functions
            string reverseFuncName = null;
            string sliceFuncName = null;
            string charSwapFuncName = null;

            // Analyze the function body to determine the names of scrambling functions
            foreach (var line in funcLines)
            {
                // Break when all functions are found
                if (reverseFuncName.IsNotBlank() && sliceFuncName.IsNotBlank() && charSwapFuncName.IsNotBlank())
                    break;

                // Get the function called on this line
                string calledFunctionName = GetFunctionCallFromLineJs(line);

                // Compose regexes to identify what function we're dealing with
                // -- reverse (1 param)
                var reverseFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\(\w+\)");
                // -- slice (return or not)
                var sliceFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.");
                // -- swap
                var swapFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b");

                // Determine the function type and assign the name
                if (reverseFuncRegex.Match(rawJs).Success)
                    reverseFuncName = calledFunctionName;
                else if (sliceFuncRegex.Match(rawJs).Success)
                    sliceFuncName = calledFunctionName;
                else if (swapFuncRegex.Match(rawJs).Success)
                    charSwapFuncName = calledFunctionName;
            }

            // Analyze the function body again to determine the operation set and order
            foreach (var line in funcLines)
            {
                // Get the function called on this line
                string calledFunctionName = GetFunctionCallFromLineJs(line);

                // Swap operation
                if (calledFunctionName.EqualsInvariant(charSwapFuncName))
                {
                    int index = Regex.Match(line, @"\(\w+,(\d+)\)").Groups[1].Value.ParseIntOrDefault();
                    yield return new SwapScramblingOperation(index);
                }
                // Slice operation
                else if (calledFunctionName.EqualsInvariant(sliceFuncName))
                {
                    int index = Regex.Match(line, @"\(\w+,(\d+)\)").Groups[1].Value.ParseIntOrDefault();
                    yield return new SliceScramblingOperation(index);
                }
                // Reverse operation
                else if (calledFunctionName.EqualsInvariant(reverseFuncName))
                {
                    yield return new ReverseScramblingOperation();
                }
            }
        }

        public static VideoInfo ParseVideoInfoJson(string rawJson)
        {
            if (rawJson.IsBlank())
                throw new ArgumentNullException(nameof(rawJson));

            // Get the json
            var json = SimpleJson.DeserializeObject(rawJson) as JsonObject;
            if (json == null)
                throw new Exception("Could not deserialize video info JSON");

            // Prepare result
            var result = new VideoInfo();

            // Try to extract player version
            var assets = json.GetOrDefault("assets") as JsonObject;
            string playerJsUrl = assets?.GetOrDefault("js", "");
            if (playerJsUrl.IsNotBlank())
            {
                var match = Regex.Match(playerJsUrl, @"player-(.+?)/");
                if (match.Success)
                    result.PlayerVersion = match.Groups[1].Value;
            }

            // Get video info
            var videoInfoEncoded = json.GetOrDefault("args") as JsonObject;
            if (videoInfoEncoded == null)
                throw new Exception("Actual video info not found in JSON");

            // Check the status
            string status = videoInfoEncoded.GetOrDefault("status", "");
            string reason = videoInfoEncoded.GetOrDefault("reason", "");
            if (status.EqualsInvariant("fail"))
                throw new YoutubeErrorException(reason);

            // Populate data
            result.Id = videoInfoEncoded.GetOrDefault("video_id", "");
            result.Title = videoInfoEncoded.GetOrDefault("title", "");
            result.Author = videoInfoEncoded.GetOrDefault("author", "");
            result.Thumbnail = videoInfoEncoded.GetOrDefault("thumbnail_url", "");
            result.ImageHighQuality = videoInfoEncoded.GetOrDefault("iurlhq", "");
            result.ImageMediumQuality = videoInfoEncoded.GetOrDefault("iurlmq", "");
            result.ImageLowQuality = videoInfoEncoded.GetOrDefault("iurlsd", "");
            result.Watermarks = videoInfoEncoded.GetOrDefault("watermark", "").Split(",");
            result.Length = TimeSpan.FromSeconds(videoInfoEncoded.GetOrDefault("length_seconds", 0.0));
            result.IsListed = videoInfoEncoded.GetOrDefault("is_listed", 1) == 1;
            result.IsRatingAllowed = videoInfoEncoded.GetOrDefault("allow_ratings", 1) == 1;
            result.IsMuted = videoInfoEncoded.GetOrDefault("muted", 0) == 1;
            result.IsEmbeddingAllowed = videoInfoEncoded.GetOrDefault("allow_embed", 1) == 1;
            result.HasClosedCaptions = videoInfoEncoded.ContainsKey("caption_audio_tracks");
            result.ViewCount = videoInfoEncoded.GetOrDefault("view_count", 0ul);
            result.AverageRating = videoInfoEncoded.GetOrDefault("avg_rating", 0.0);
            result.Keywords = videoInfoEncoded.GetOrDefault("keywords", "").Split(",");

            // Get the streams
            string streamsRaw = videoInfoEncoded.GetOrDefault("adaptive_fmts", "");
            if (streamsRaw.IsBlank())
                streamsRaw = videoInfoEncoded.GetOrDefault("url_encoded_fmt_stream_map", "");
            result.Streams = ParseVideoStreamInfosUrlEncoded(streamsRaw).ToArray();

            // Check if any of the streams need to be deciphered
            result.NeedsDeciphering = result.Streams.Any(s => s.NeedsDeciphering);

            // Return
            return result;
        }

        public static VideoInfo ParseVideoInfoUrlEncoded(string rawUrlEncoded)
        {
            if (rawUrlEncoded.IsBlank())
                throw new ArgumentNullException(nameof(rawUrlEncoded));

            // Get data
            var videoInfoEncoded = ParseDictionaryUrlEncoded(rawUrlEncoded);

            // Check the status
            if (videoInfoEncoded.GetOrDefault("status").EqualsInvariant("fail"))
                throw new YoutubeErrorException(videoInfoEncoded.GetOrDefault("reason"));

            // Prepare result
            var result = new VideoInfo();

            // Player version
            result.PlayerVersion = null; // set to null, so that decipherer can correctly throw an exception

            // Populate data
            result.Id = videoInfoEncoded.GetOrDefault("video_id");
            result.Title = videoInfoEncoded.GetOrDefault("title");
            result.Author = videoInfoEncoded.GetOrDefault("author");
            result.Thumbnail = videoInfoEncoded.GetOrDefault("thumbnail_url");
            result.ImageHighQuality = videoInfoEncoded.GetOrDefault("iurlhq");
            result.ImageMediumQuality = videoInfoEncoded.GetOrDefault("iurlmq");
            result.ImageLowQuality = videoInfoEncoded.GetOrDefault("iurlsd");
            result.Watermarks = videoInfoEncoded.GetOrDefault("watermark").Split(",");
            result.Length = TimeSpan.FromSeconds(videoInfoEncoded.GetOrDefault("length_seconds").ParseDoubleOrDefault());
            result.IsListed = videoInfoEncoded.GetOrDefault("is_listed").ParseIntOrDefault(1) == 1;
            result.IsRatingAllowed = videoInfoEncoded.GetOrDefault("allow_ratings").ParseIntOrDefault(1) == 1;
            result.IsMuted = videoInfoEncoded.GetOrDefault("muted").ParseIntOrDefault() == 1;
            result.IsEmbeddingAllowed = videoInfoEncoded.GetOrDefault("allow_embed").ParseIntOrDefault(1) == 1;
            result.HasClosedCaptions = videoInfoEncoded.ContainsKey("caption_audio_tracks");
            result.ViewCount = videoInfoEncoded.GetOrDefault("view_count").ParseUlongOrDefault();
            result.AverageRating = videoInfoEncoded.GetOrDefault("avg_rating").ParseDoubleOrDefault();
            result.Keywords = videoInfoEncoded.GetOrDefault("keywords").Split(",");

            // Get the streams
            string streamsRaw = videoInfoEncoded.GetOrDefault("adaptive_fmts");
            if (streamsRaw.IsBlank())
                streamsRaw = videoInfoEncoded.GetOrDefault("url_encoded_fmt_stream_map");
            result.Streams = ParseVideoStreamInfosUrlEncoded(streamsRaw).ToArray();

            // Check if any of the streams need to be deciphered
            result.NeedsDeciphering = result.Streams.Any(s => s.NeedsDeciphering);

            // Return
            return result;
        }

        public static PlayerSource ParsePlayerSourceJs(string rawJs)
        {
            if (rawJs.IsBlank())
                throw new ArgumentNullException(nameof(rawJs));

            var result = new PlayerSource();
            result.ScramblingOperations = ParseScramblingOperationsJs(rawJs).ToArray();

            return result;
        }
    }
}