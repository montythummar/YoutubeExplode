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
            var videoInfoJson = json.GetValueOrDefault("args") as JsonObject;
            if (videoInfoJson == null)
                throw new Exception("Video info not found in JSON");

            // Check the status
            string status = videoInfoJson.GetValueOrDefault("status", "");
            string reason = videoInfoJson.GetValueOrDefault("reason", "");
            if (status.EqualsInvariant("fail"))
                throw new YoutubeErrorException(reason);

            // Populate data
            result.ID = videoInfoJson.GetValueOrDefault("video_id", "");
            result.Title = videoInfoJson.GetValueOrDefault("title", "");
            result.Author = videoInfoJson.GetValueOrDefault("author", "");
            result.Thumbnail = videoInfoJson.GetValueOrDefault("thumbnail_url", "");
            result.ImageHighQuality = videoInfoJson.GetValueOrDefault("iurlhq", "");
            result.ImageMediumQuality = videoInfoJson.GetValueOrDefault("iurlmq", "");
            result.ImageLowQuality = videoInfoJson.GetValueOrDefault("iurlsd", "");
            result.Watermarks = videoInfoJson.GetValueOrDefault("watermark", "").Split(",");
            result.Length = TimeSpan.FromSeconds(videoInfoJson.GetValueOrDefault("length_seconds", 0.0));
            result.IsListed = videoInfoJson.GetValueOrDefault("is_listed", 1) == 1;
            result.IsRatingAllowed = videoInfoJson.GetValueOrDefault("allow_ratings", 1) == 1;
            result.IsMuted = videoInfoJson.GetValueOrDefault("muted", 0) == 1;
            result.IsEmbeddingAllowed = videoInfoJson.GetValueOrDefault("allow_embed", 1) == 1;
            result.HasClosedCaptions = videoInfoJson.ContainsKey("caption_audio_tracks");
            result.ViewCount = videoInfoJson.GetValueOrDefault("view_count", 0ul);
            result.AverageRating = videoInfoJson.GetValueOrDefault("avg_rating", 0.0);
            result.Keywords = videoInfoJson.GetValueOrDefault("keywords", "").Split(",");

            // Get the streams
            string streamsRaw = videoInfoJson.GetValueOrDefault("adaptive_fmts", "");
            if (streamsRaw.IsBlank())
                streamsRaw = videoInfoJson.GetValueOrDefault("url_encoded_fmt_stream_map", "");
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
            var urlEncodedDic = ParseDictionaryUrlEncoded(urlRaw);

            // Check the status
            if (urlEncodedDic.GetValueOrDefault("status").EqualsInvariant("fail"))
                throw new YoutubeErrorException(urlEncodedDic.GetValueOrDefault("reason"));

            // Prepare result
            var result = new VideoInfo();

            // Populate data
            result.ID = urlEncodedDic.GetValueOrDefault("video_id");
            result.Title = urlEncodedDic.GetValueOrDefault("title");
            result.Author = urlEncodedDic.GetValueOrDefault("author");
            result.Thumbnail = urlEncodedDic.GetValueOrDefault("thumbnail_url");
            result.ImageHighQuality = urlEncodedDic.GetValueOrDefault("iurlhq");
            result.ImageMediumQuality = urlEncodedDic.GetValueOrDefault("iurlmq");
            result.ImageLowQuality = urlEncodedDic.GetValueOrDefault("iurlsd");
            result.Watermarks = urlEncodedDic.GetValueOrDefault("watermark").Split(",");
            result.Length = TimeSpan.FromSeconds(urlEncodedDic.GetValueOrDefault("length_seconds").ParseDoubleOrDefault());
            result.IsListed = urlEncodedDic.GetValueOrDefault("is_listed").ParseIntOrDefault(1) == 1;
            result.IsRatingAllowed = urlEncodedDic.GetValueOrDefault("allow_ratings").ParseIntOrDefault(1) == 1;
            result.IsMuted = urlEncodedDic.GetValueOrDefault("muted").ParseIntOrDefault() == 1;
            result.IsEmbeddingAllowed = urlEncodedDic.GetValueOrDefault("allow_embed").ParseIntOrDefault(1) == 1;
            result.HasClosedCaptions = urlEncodedDic.ContainsKey("caption_audio_tracks");
            result.ViewCount = urlEncodedDic.GetValueOrDefault("view_count").ParseUlongOrDefault();
            result.AverageRating = urlEncodedDic.GetValueOrDefault("avg_rating").ParseDoubleOrDefault();
            result.Keywords = urlEncodedDic.GetValueOrDefault("keywords").Split(",");

            // Get the streams
            string streamsRaw = urlEncodedDic.GetValueOrDefault("adaptive_fmts");
            if (streamsRaw.IsBlank())
                streamsRaw = urlEncodedDic.GetValueOrDefault("url_encoded_fmt_stream_map");
            result.Streams = ParseVideoStreamEndpointsUrlEncoded(streamsRaw).ToArray();

            // Check if any of the streams need to be deciphered
            result.NeedsDeciphering = result.Streams.Any(s => s.NeedsDeciphering);

            // Return
            return result;
        }
    }
}