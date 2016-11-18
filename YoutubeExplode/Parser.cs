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
using YoutubeExplode.Exceptions;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    internal static class Parser
    {
        private static Dictionary<string, string> GetParameters(string parametersRaw)
        {
            var dic = new Dictionary<string, string>();
            var keyValuePairsRaw = parametersRaw.Split("&");
            foreach (string keyValuePairRaw in keyValuePairsRaw)
            {
                string keyValuePairRawDecoded = WebUtility.UrlDecode(keyValuePairRaw);
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
                string sig = streamsDic.GetValueOrDefault("signature") ??
                             streamsDic.GetValueOrDefault("sig") ??
                             streamsDic.GetValueOrDefault("s");
                string url = streamsDic.GetValueOrDefault("url");
                string type = streamsDic.GetValueOrDefault("type");
                string quality = streamsDic.GetValueOrDefault("quality_label") ?? streamsDic.GetValueOrDefault("quality");
                string resolution = streamsDic.GetValueOrDefault("size");
                int bitrate = streamsDic.GetValueOrDefault("bitrate").ParseIntOrDefault();
                double fps = streamsDic.GetValueOrDefault("fps").ParseDoubleOrDefault();

                // Add stream
                yield return new VideoStreamEndpoint
                {
                    Signature = sig,
                    URL = url,
                    TypeString = type,
                    QualityString = quality,
                    ResolutionString = resolution,
                    Bitrate = bitrate,
                    FPS = fps
                };
            }
        }

        public static VideoInfo ParseVideoInfo(string infoRaw)
        {
            // Check arguments
            if (string.IsNullOrWhiteSpace(infoRaw))
                throw new ArgumentNullException(nameof(infoRaw));

            // Get parameters
            var dic = GetParameters(infoRaw);

            // Check for error
            if (dic.GetValueOrDefault("status").EqualsInvariant("fail"))
                throw new YoutubeErrorException(dic.GetValueOrDefault("reason"));

            // Set basic values (first layer)
            var result = new VideoInfo
            {
                ID = dic.GetValueOrDefault("video_id"),
                Title = dic.GetValueOrDefault("title"),
                Author = dic.GetValueOrDefault("author"),
                Thumbnail = dic.GetValueOrDefault("thumbnail_url"),
                ImageHighQuality = dic.GetValueOrDefault("iurlhq"),
                ImageMediumQuality = dic.GetValueOrDefault("iurlmq"),
                ImageLowQuality = dic.GetValueOrDefault("iurlsd"),
                Watermarks = dic.GetValueOrDefault("watermark")?.Split(","),
                Length = TimeSpan.FromSeconds(dic.GetValueOrDefault("length_seconds").ParseDoubleOrDefault()),
                IsListed = dic.GetValueOrDefault("is_listed").ParseIntOrDefault() == 1,
                IsRatingAllowed = dic.GetValueOrDefault("allow_ratings").ParseIntOrDefault() == 1,
                IsMuted = dic.GetValueOrDefault("muted").ParseIntOrDefault() == 1,
                IsEmbedingAllowed = dic.GetValueOrDefault("allow_embed").ParseIntOrDefault() == 1,
                HasClosedCaptions = dic.GetValueOrDefault("has_cc").EqualsInvariant("true"),
                ViewCount = dic.GetValueOrDefault("view_count").ParseIntOrDefault(),
                AvgRating = dic.GetValueOrDefault("avg_rating").ParseDoubleOrDefault(),
                Keywords = dic.GetValueOrDefault("keywords")?.Split(","),
                NeedsDeciphering = dic.GetValueOrDefault("use_cipher_signature").EqualsInvariant("true")
            };

            // Get the streams
            string streamsRaw = dic.GetValueOrDefault("adaptive_fmts") ?? dic.GetValueOrDefault("url_encoded_fmt_stream_map");
            if (!string.IsNullOrWhiteSpace(streamsRaw))
                result.Streams = ParseVideoStreamEndpoints(streamsRaw).ToArray();

            // Return
            return result;
        }
    }
}