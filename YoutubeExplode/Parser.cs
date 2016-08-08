// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <Parser.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    internal static class Parser
    {
        public static VideoInfo ParseVideoInfo(string rawInfo)
        {
            // Check arguments
            if (string.IsNullOrWhiteSpace(rawInfo))
                return null;

            // Double URL decode everything
            rawInfo = HttpUtility.UrlDecode(rawInfo);
            rawInfo = HttpUtility.UrlDecode(rawInfo);
            if (rawInfo == null) return null;

            // Split the raw info into rows
            var rows = rawInfo.Split(new[] {"&"}, StringSplitOptions.RemoveEmptyEntries);

            // Split the rows further into a dictionary
            var dic = new Dictionary<string, string>();
            foreach (string row in rows)
            {
                // Look for the equals sign
                int equalsPos = row.IndexOf('=');
                if (equalsPos < 0) continue;

                // Get the key and value
                string key = row.Substring(0, equalsPos);
                string value = row.Substring(equalsPos + 1);

                // Add to dictionary
                dic.Add(key, value);
            }

            // Prepare the returned object by setting basic values
            var result = new VideoInfo
            {
                ID = dic.GetValueOrDefault("video_id"),

                Title = dic.GetValueOrDefault("title"),
                Author = dic.GetValueOrDefault("author"),
                ThumbnailURL = dic.GetValueOrDefault("iurl"),
                Length = TimeSpan.FromSeconds(dic.GetValueOrDefault("length_seconds").ParseDoubleOrDefault()),

                IsListed = dic.GetValueOrDefault("is_listed").ParseIntOrDefault() == 1,

                ViewCount = dic.GetValueOrDefault("view_count").ParseIntOrDefault(),
                AvgRating = dic.GetValueOrDefault("avg_rating").ParseDoubleOrDefault()
            };

            // Get the streams
            string streamsRaw = dic.GetValueOrDefault("url_encoded_fmt_stream_map");
            if (string.IsNullOrWhiteSpace(streamsRaw)) return result;
            var streams = new List<VideoStreamEndpoint>();
            foreach (var streamRaw in streamsRaw.Split(new[] {",url="}, StringSplitOptions.RemoveEmptyEntries))
            {
                // Extract data
                string type = Regex.Match(streamRaw, @"type=(.*)(?:;)").Value;
                string quality = Regex.Match(streamRaw, @"quality=(.*)(?:\b|&)").Value;
                string url = streamRaw.Replace("url=", "").Replace(" ", "%20");

                // Add stream
                streams.Add(new VideoStreamEndpoint
                {
                    Type = type,
                    Quality = quality,
                    URL = url
                });
            }
            result.Streams = streams;

            // Return
            return result;
        }
    }
}