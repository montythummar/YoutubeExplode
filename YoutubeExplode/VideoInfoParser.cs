// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <VideoInfoParser.cs>
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
    internal static class VideoInfoParser
    {
        private static Dictionary<string, string> ParseDictionaryUrlEncoded(string rawUrlEncoded)
        {
            if (rawUrlEncoded.IsBlank())
                return null;

            var dic = new Dictionary<string, string>();
            var keyValuePairsRaw = rawUrlEncoded.Split("&");
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

        private static IEnumerable<VideoStream> ParseVideoStreamsUrlEncoded(string rawUrlEncoded)
        {
            if (rawUrlEncoded.IsBlank())
                yield break;

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
                yield return new VideoStream
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
                var match = Regex.Match(playerJsUrl, @"player-(.+?)/",
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
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
            result.KeywordsString = videoInfoEncoded.GetOrDefault("keywords", "");

            // Get the streams
            string streamsRaw = videoInfoEncoded.GetOrDefault("adaptive_fmts", "");
            if (streamsRaw.IsBlank())
                streamsRaw = videoInfoEncoded.GetOrDefault("url_encoded_fmt_stream_map", "");
            result.Streams = ParseVideoStreamsUrlEncoded(streamsRaw).ToArray();

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
            result.KeywordsString = videoInfoEncoded.GetOrDefault("keywords");

            // Get the streams
            string streamsRaw = videoInfoEncoded.GetOrDefault("adaptive_fmts");
            if (streamsRaw.IsBlank())
                streamsRaw = videoInfoEncoded.GetOrDefault("url_encoded_fmt_stream_map");
            result.Streams = ParseVideoStreamsUrlEncoded(streamsRaw).ToArray();

            // Check if any of the streams need to be deciphered
            result.NeedsDeciphering = result.Streams.Any(s => s.NeedsDeciphering);

            // Return
            return result;
        }
    }
}