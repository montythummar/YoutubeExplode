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

            // Split the raw info into rows
            var rawRows = rawInfo.Split(new[] {"&"}, StringSplitOptions.RemoveEmptyEntries);

            // Split the rows further into a dictionary
            var dic = new Dictionary<string, string>();
            foreach (string rawRow in rawRows)
            {
                // Decode
                string row = HttpUtility.UrlDecode(rawRow);
                if (row == null) continue;

                // Look for the equals sign
                int equalsPos = row.IndexOf('=');
                if (equalsPos < 0) continue;

                // Get the key and value
                string key = row.Substring(0, equalsPos);
                string value = row.Substring(equalsPos + 1);

                // Add to dictionary
                dic[key] = value;
            }

            // Check for error
            if (dic.GetValueOrDefault("status") == "fail")
                throw new Exception($"Youtube returned an error: {dic.GetValueOrDefault("reason")}");

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
            foreach (var streamRaw in streamsRaw.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries))
            {
                // Decode
                string stream = HttpUtility.UrlDecode(streamRaw);
                if (stream == null) continue;

                // Extract data
                string type = Regex.Match(stream, @"type=(.+?);").Groups[1].Value;
                string quality = Regex.Match(stream, @"quality=(.+?)\b").Groups[1].Value;

                int urlPos = stream.IndexOf("url=", StringComparison.Ordinal);
                string url = stream.Substring(urlPos + 4).Replace(" ", "%20");

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