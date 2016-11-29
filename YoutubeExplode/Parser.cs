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
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex VideoPlayerVersionRegex = new Regex(@"player-(.+?)/",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Dictionary<string, string> GetParameters(string parametersRaw)
        {
            var dic = new Dictionary<string, string>();
            var keyValuePairsRaw = parametersRaw.Split("&");
            foreach (string keyValuePairRaw in keyValuePairsRaw)
            {
                string keyValuePairRawDecoded = Uri.UnescapeDataString(keyValuePairRaw);
                if (string.IsNullOrWhiteSpace(keyValuePairRawDecoded))
                    continue;

                // Look for the equals sign
                int equalsPos = keyValuePairRawDecoded.IndexOf('=');
                if (equalsPos <= 0)
                    continue;

                // Get the key and value
                string key = keyValuePairRawDecoded.Substring(0, equalsPos);
                string value = equalsPos < keyValuePairRawDecoded.Length
                    ? keyValuePairRawDecoded.Substring(equalsPos + 1)
                    : string.Empty;

                // Add to dictionary
                dic[key] = value;
            }

            return dic;
        }

        private static IEnumerable<VideoStreamEndpoint> ParseVideoStreamEndpoints(string streamsRaw)
        {
            foreach (var streamRaw in streamsRaw.Split(","))
            {
                var streamsDic = GetParameters(streamRaw);

                // Extract values
                string sig = streamsDic.GetValueOrDefault("s") ??
                             streamsDic.GetValueOrDefault("sig") ??
                             streamsDic.GetValueOrDefault("signature");
                bool needsDeciphering = streamsDic.ContainsKey("s");
                string url = streamsDic.GetValueOrDefault("url");
                string type = streamsDic.GetValueOrDefault("type");
                string quality = streamsDic.GetValueOrDefault("quality_label") ??
                                 streamsDic.GetValueOrDefault("quality");
                string resolution = streamsDic.GetValueOrDefault("size");
                int bitrate = streamsDic.GetValueOrDefault("bitrate").ParseIntOrDefault();
                double fps = streamsDic.GetValueOrDefault("fps").ParseDoubleOrDefault();

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
            if (string.IsNullOrWhiteSpace(html))
                throw new ArgumentNullException(nameof(html));

            // Get the json
            var jsonMatch = VideoJsonRegex.Match(html);
            if (!jsonMatch.Success)
                throw new Exception("Could not find the video info JSON on the video page");
            string jsonString = jsonMatch.Groups[1].Value;
            var json = ParseJson(jsonString);
            if (json == null)
                throw new Exception("Could not deserialize video info JSON");

            // Prepare result
            var result = new VideoInfo();

            // Extract player version
            var assets = json.GetValueOrDefault("assets") as JsonObject;
            string playerJsUrl = assets?.GetValueOrDefault("js").ConvertOrDefault(string.Empty);
            if (!string.IsNullOrWhiteSpace(playerJsUrl))
                result.PlayerVersion = VideoPlayerVersionRegex.Match(playerJsUrl).Groups[1].Value;

            // Get video info
            var videoInfo = json.GetValueOrDefault("args") as JsonObject;
            if (videoInfo == null)
                throw new Exception("Video info not found in JSON");

            // Check the status
            string status = videoInfo.GetValueOrDefault("status").ConvertOrDefault(string.Empty);
            string reason = videoInfo.GetValueOrDefault("reason").ConvertOrDefault(string.Empty);
            if (status.EqualsInvariant("fail"))
                throw new YoutubeErrorException(reason);

            // Set basic values from video info
            result.ID = videoInfo.GetValueOrDefault("video_id").ConvertOrDefault(string.Empty);
            result.Title = videoInfo.GetValueOrDefault("title").ConvertOrDefault(string.Empty);
            result.Author = videoInfo.GetValueOrDefault("author").ConvertOrDefault(string.Empty);
            result.Thumbnail = videoInfo.GetValueOrDefault("thumbnail_url").ConvertOrDefault(string.Empty);
            result.ImageHighQuality = videoInfo.GetValueOrDefault("iurlhq").ConvertOrDefault(string.Empty);
            result.ImageMediumQuality = videoInfo.GetValueOrDefault("iurlmq").ConvertOrDefault(string.Empty);
            result.ImageLowQuality = videoInfo.GetValueOrDefault("iurlsd").ConvertOrDefault(string.Empty);
            result.Watermarks = videoInfo.GetValueOrDefault("watermark").ConvertOrDefault(string.Empty).Split(",");
            result.Length = TimeSpan.FromSeconds(videoInfo.GetValueOrDefault("length_seconds").ConvertOrDefault(0.0));
            result.IsListed = videoInfo.GetValueOrDefault("is_listed").ConvertOrDefault(1) == 1;
            result.IsRatingAllowed = videoInfo.GetValueOrDefault("allow_ratings").ConvertOrDefault(1) == 1;
            result.IsMuted = videoInfo.GetValueOrDefault("muted").ConvertOrDefault(0) == 1;
            result.IsEmbedingAllowed = videoInfo.GetValueOrDefault("allow_embed").ConvertOrDefault(1) == 1;
            result.HasClosedCaptions = videoInfo.GetValueOrDefault("caption_audio_tracks") != null;
            result.ViewCount = videoInfo.GetValueOrDefault("view_count").ConvertOrDefault(0u);
            result.AvgRating = videoInfo.GetValueOrDefault("avg_rating").ConvertOrDefault(0.0);
            result.Keywords = videoInfo.GetValueOrDefault("keywords").ConvertOrDefault(string.Empty).Split(",");

            // Get the streams
            string streamsRaw =
                (videoInfo.GetValueOrDefault("adaptive_fmts") ??
                 videoInfo.GetValueOrDefault("url_encoded_fmt_stream_map"))
                    .ConvertOrDefault(string.Empty);
            result.Streams = !string.IsNullOrWhiteSpace(streamsRaw)
                ? ParseVideoStreamEndpoints(streamsRaw).ToArray()
                : Enumerable.Empty<VideoStreamEndpoint>().ToArray();

            // Signature
            result.NeedsDeciphering = result.Streams.Any(s => s.NeedsDeciphering);

            // Return
            return result;
        }
    }
}