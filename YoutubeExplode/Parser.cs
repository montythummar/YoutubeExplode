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
using System.Net;
using System.Text.RegularExpressions;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    internal static class Parser
    {
        private static readonly Regex VideoPlayerVersionRegex = new Regex(@"player-(.+?)/",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Dictionary<string, string> ParseDictionaryUrlEncoded(string raw)
        {
            if (raw.IsBlank())
                return null;

            var dic = new Dictionary<string, string>();
            var keyValuePairsRaw = raw.Split("&");
            foreach (string keyValuePairRaw in keyValuePairsRaw)
            {
                string keyValuePairRawDecoded = WebUtility.UrlDecode(keyValuePairRaw);
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

        private static IEnumerable<VideoStreamEndpoint> ParseVideoStreamEndpointsUrlEncoded(string raw)
        {
            if (raw.IsBlank())
                yield break;

            foreach (var streamRaw in raw.Split(","))
            {
                var dic = ParseDictionaryUrlEncoded(streamRaw);

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

        public static VideoInfo ParseVideoInfoJson(string rawJson)
        {
            if (rawJson.IsBlank())
                throw new ArgumentNullException(nameof(rawJson));

            // Get the json
            var json = ParseJson(rawJson);
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
            var videoInfoEncoded = json.GetValueOrDefault("args") as JsonObject;
            if (videoInfoEncoded == null)
                throw new Exception("Video info not found in JSON");

            // Check the status
            string status = videoInfoEncoded.GetValueOrDefault("status", "");
            string reason = videoInfoEncoded.GetValueOrDefault("reason", "");
            if (status.EqualsInvariant("fail"))
                throw new YoutubeErrorException(reason);

            // Populate data
            result.ID = videoInfoEncoded.GetValueOrDefault("video_id", "");
            result.Title = videoInfoEncoded.GetValueOrDefault("title", "");
            result.Author = videoInfoEncoded.GetValueOrDefault("author", "");
            result.Thumbnail = videoInfoEncoded.GetValueOrDefault("thumbnail_url", "");
            result.ImageHighQuality = videoInfoEncoded.GetValueOrDefault("iurlhq", "");
            result.ImageMediumQuality = videoInfoEncoded.GetValueOrDefault("iurlmq", "");
            result.ImageLowQuality = videoInfoEncoded.GetValueOrDefault("iurlsd", "");
            result.Watermarks = videoInfoEncoded.GetValueOrDefault("watermark", "").Split(",");
            result.Length = TimeSpan.FromSeconds(videoInfoEncoded.GetValueOrDefault("length_seconds", 0.0));
            result.IsListed = videoInfoEncoded.GetValueOrDefault("is_listed", 1) == 1;
            result.IsRatingAllowed = videoInfoEncoded.GetValueOrDefault("allow_ratings", 1) == 1;
            result.IsMuted = videoInfoEncoded.GetValueOrDefault("muted", 0) == 1;
            result.IsEmbeddingAllowed = videoInfoEncoded.GetValueOrDefault("allow_embed", 1) == 1;
            result.HasClosedCaptions = videoInfoEncoded.ContainsKey("caption_audio_tracks");
            result.ViewCount = videoInfoEncoded.GetValueOrDefault("view_count", 0ul);
            result.AverageRating = videoInfoEncoded.GetValueOrDefault("avg_rating", 0.0);
            result.Keywords = videoInfoEncoded.GetValueOrDefault("keywords", "").Split(",");

            // Get the streams
            string streamsRaw = videoInfoEncoded.GetValueOrDefault("adaptive_fmts", "");
            if (streamsRaw.IsBlank())
                streamsRaw = videoInfoEncoded.GetValueOrDefault("url_encoded_fmt_stream_map", "");
            result.Streams = ParseVideoStreamEndpointsUrlEncoded(streamsRaw).ToArray();

            // Check if any of the streams need to be deciphered
            result.NeedsDeciphering = result.Streams.Any(s => s.NeedsDeciphering);

            // Return
            return result;
        }

        public static VideoInfo ParseVideoInfoUrlEncoded(string urlRaw)
        {
            if (urlRaw.IsBlank())
                throw new ArgumentNullException(nameof(urlRaw));

            // Get data
            var videoInfoEncoded = ParseDictionaryUrlEncoded(urlRaw);

            // Check the status
            if (videoInfoEncoded.GetValueOrDefault("status").EqualsInvariant("fail"))
                throw new YoutubeErrorException(videoInfoEncoded.GetValueOrDefault("reason"));

            // Prepare result
            var result = new VideoInfo();

            // Player version (no way to obtain here)
            result.PlayerVersion = null; // set to null, so deciphere can correctly throw an exception

            // Populate data
            result.ID = videoInfoEncoded.GetValueOrDefault("video_id");
            result.Title = videoInfoEncoded.GetValueOrDefault("title");
            result.Author = videoInfoEncoded.GetValueOrDefault("author");
            result.Thumbnail = videoInfoEncoded.GetValueOrDefault("thumbnail_url");
            result.ImageHighQuality = videoInfoEncoded.GetValueOrDefault("iurlhq");
            result.ImageMediumQuality = videoInfoEncoded.GetValueOrDefault("iurlmq");
            result.ImageLowQuality = videoInfoEncoded.GetValueOrDefault("iurlsd");
            result.Watermarks = videoInfoEncoded.GetValueOrDefault("watermark").Split(",");
            result.Length = TimeSpan.FromSeconds(videoInfoEncoded.GetValueOrDefault("length_seconds").ParseDoubleOrDefault());
            result.IsListed = videoInfoEncoded.GetValueOrDefault("is_listed").ParseIntOrDefault(1) == 1;
            result.IsRatingAllowed = videoInfoEncoded.GetValueOrDefault("allow_ratings").ParseIntOrDefault(1) == 1;
            result.IsMuted = videoInfoEncoded.GetValueOrDefault("muted").ParseIntOrDefault() == 1;
            result.IsEmbeddingAllowed = videoInfoEncoded.GetValueOrDefault("allow_embed").ParseIntOrDefault(1) == 1;
            result.HasClosedCaptions = videoInfoEncoded.ContainsKey("caption_audio_tracks");
            result.ViewCount = videoInfoEncoded.GetValueOrDefault("view_count").ParseUlongOrDefault();
            result.AverageRating = videoInfoEncoded.GetValueOrDefault("avg_rating").ParseDoubleOrDefault();
            result.Keywords = videoInfoEncoded.GetValueOrDefault("keywords").Split(",");

            // Get the streams
            string streamsRaw = videoInfoEncoded.GetValueOrDefault("adaptive_fmts");
            if (streamsRaw.IsBlank())
                streamsRaw = videoInfoEncoded.GetValueOrDefault("url_encoded_fmt_stream_map");
            result.Streams = ParseVideoStreamEndpointsUrlEncoded(streamsRaw).ToArray();

            // Check if any of the streams need to be deciphered
            result.NeedsDeciphering = result.Streams.Any(s => s.NeedsDeciphering);

            // Return
            return result;
        }
    }
}