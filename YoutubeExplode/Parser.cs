// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <Parser.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Web;
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
                string keyValuePairRawDecoded = HttpUtility.UrlDecode(keyValuePairRaw);
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

        public static VideoInfo ParseVideoInfo(string infoRaw)
        {
            // Check arguments
            if (string.IsNullOrWhiteSpace(infoRaw))
                throw new ArgumentException("infoRaw should not be null or empty", nameof(infoRaw));

            // Get parameters
            var dic = GetParameters(infoRaw);

            // Check for error
            if (dic.GetValueOrDefault("status").Equals("fail", StringComparison.InvariantCultureIgnoreCase))
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
                HasClosedCaptions = dic.GetValueOrDefault("has_cc").Equals("true", StringComparison.InvariantCultureIgnoreCase),
                ViewCount = dic.GetValueOrDefault("view_count").ParseIntOrDefault(),
                AvgRating = dic.GetValueOrDefault("avg_rating").ParseDoubleOrDefault(),
                Keywords = dic.GetValueOrDefault("keywords")?.Split(","),
                UseCipherSignature = dic.GetValueOrDefault("use_cipher_signature").Equals("true", StringComparison.InvariantCultureIgnoreCase)
            };

            // Get the streams
            string streamsRaw = dic.GetValueOrDefault("adaptive_fmts") ?? dic.GetValueOrDefault("url_encoded_fmt_stream_map");
            if (string.IsNullOrWhiteSpace(streamsRaw)) return result;
            var streams = new List<VideoStreamEndpoint>();
            foreach (var streamRaw in streamsRaw.Split(","))
            {
                var streamsDic = GetParameters(streamRaw);

                // Extract values
                string type = streamsDic.GetValueOrDefault("type");
                string quality = streamsDic.GetValueOrDefault("quality_label") ?? streamsDic.GetValueOrDefault("quality");
                string url = streamsDic.GetValueOrDefault("url");
                string resolution = streamsDic.GetValueOrDefault("size");
                int bitrate = streamsDic.GetValueOrDefault("bitrate").ParseIntOrDefault();
                double fps = streamsDic.GetValueOrDefault("fps").ParseDoubleOrDefault();

                // Add stream
                streams.Add(new VideoStreamEndpoint
                {
                    URL = url,
                    TypeString = type,
                    QualityString = quality,
                    ResolutionString = resolution,
                    Bitrate = bitrate,
                    FPS = fps
                });
            }
            result.Streams = streams.ToArray();

            // Return
            return result;
        }
    }
}