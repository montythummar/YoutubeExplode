// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <Parser.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    internal static class Parser
    {
        private static readonly Regex VideoJsonRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex VideoPlayerVersionRegex = new Regex(@"player-(.+?)/",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Dictionary<string, string> ParseEncodedDictionary(string raw)
        {
            if (raw.IsBlank())
                return null;

            var dic = new Dictionary<string, string>();
            var keyValuePairsRaw = raw.Split("&");
            foreach (string keyValuePairRaw in keyValuePairsRaw)
            {
                string keyValuePairRawDecoded = Uri.UnescapeDataString(keyValuePairRaw);
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

        private static IEnumerable<VideoStreamEndpoint> ParseVideoStreamEndpoints(string raw)
        {
            if (raw.IsBlank())
                yield break;

            foreach (var streamRaw in raw.Split(","))
            {
                var dic = ParseEncodedDictionary(streamRaw);

                // Extract values
                string sig = dic.GetValueOrDefault("s");
                bool needsDeciphering = !sig.IsBlank();
                string url = dic.GetValueOrDefault("url");
                string type = dic.GetValueOrDefault("type");
                string quality = dic.GetValueOrDefault("quality_label") ??
                                 dic.GetValueOrDefault("quality");
                string resolution = dic.GetValueOrDefault("size") ?? "0x0";
                ulong bitrate = dic.GetValueOrDefault("bitrate").ParseUlongOrDefault();
                double fps = dic.GetValueOrDefault("fps").ParseDoubleOrDefault();

                // Yield a stream object
                yield return new VideoStreamEndpoint
                {
                    Signature = sig,
                    NeedsDeciphering = needsDeciphering,
                    URL = url,
                    TypeString = type,
                    QualityString = quality,
                    ResolutionString = resolution,
                    Bitrate = bitrate,
                    FPS = fps
                };
            }
        }

        private static JsonObject ParseJson(string json)
        {
            return SimpleJson.DeserializeObject(json) as JsonObject;
        }

        public static VideoInfo ParseVideoInfo(string html)
        {
            if (html.IsBlank())
                throw new ArgumentNullException(nameof(html));

            // Get the json
            var jsonMatch = VideoJsonRegex.Match(html);
            if (!jsonMatch.Success)
                throw new Exception("Could not find the video info JSON on the video watch page");

            string jsonString = jsonMatch.Groups[1].Value;
            var json = ParseJson(jsonString);
            if (json == null)
                throw new Exception("Could not deserialize video info JSON");

            // Prepare result
            var result = new VideoInfo();

            // Try to extract player version
            var assets = json.GetValueOrDefault("assets") as JsonObject;
            string playerJsUrl = assets?.GetValueOrDefault("js", "");
            if (!playerJsUrl.IsBlank())
                result.PlayerVersion = VideoPlayerVersionRegex.Match(playerJsUrl).Groups[1].Value;

            // Get video info
            var videoInfo = json.GetValueOrDefault("args") as JsonObject;
            if (videoInfo == null)
                throw new Exception("Video info not found in JSON");

            // Check the status
            string status = videoInfo.GetValueOrDefault("status", "");
            string reason = videoInfo.GetValueOrDefault("reason", "");
            if (status.EqualsInvariant("fail"))
                throw new YoutubeErrorException(reason);

            // Set basic values from video info
            result.ID = videoInfo.GetValueOrDefault("video_id", "");
            result.Title = videoInfo.GetValueOrDefault("title", "");
            result.Author = videoInfo.GetValueOrDefault("author", "");
            result.Thumbnail = videoInfo.GetValueOrDefault("thumbnail_url", "");
            result.ImageHighQuality = videoInfo.GetValueOrDefault("iurlhq", "");
            result.ImageMediumQuality = videoInfo.GetValueOrDefault("iurlmq", "");
            result.ImageLowQuality = videoInfo.GetValueOrDefault("iurlsd", "");
            result.Watermarks = videoInfo.GetValueOrDefault("watermark", "").Split(",");
            result.Length = TimeSpan.FromSeconds(videoInfo.GetValueOrDefault("length_seconds", 0.0));
            result.IsListed = videoInfo.GetValueOrDefault("is_listed", 1) == 1;
            result.IsRatingAllowed = videoInfo.GetValueOrDefault("allow_ratings", 1) == 1;
            result.IsMuted = videoInfo.GetValueOrDefault("muted", 0) == 1;
            result.IsEmbeddingAllowed = videoInfo.GetValueOrDefault("allow_embed", 1) == 1;
            result.HasClosedCaptions = videoInfo.ContainsKey("caption_audio_tracks");
            result.ViewCount = videoInfo.GetValueOrDefault("view_count", 0ul);
            result.AverageRating = videoInfo.GetValueOrDefault("avg_rating", 0.0);
            result.Keywords = videoInfo.GetValueOrDefault("keywords", "").Split(",");

            // Get the streams
            string streamsRaw = videoInfo.GetValueOrDefault("adaptive_fmts", "");
            if (streamsRaw.IsBlank())
                streamsRaw = videoInfo.GetValueOrDefault("url_encoded_fmt_stream_map", "");
            result.Streams = ParseVideoStreamEndpoints(streamsRaw).ToArray();

            // Check if any of the streams need to be deciphered
            result.NeedsDeciphering = result.Streams.Any(s => s.NeedsDeciphering);

            // Return
            return result;
        }
    }
}