using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using YoutubeExplode.Exceptions;
using YoutubeExplode.Models;

namespace YoutubeExplode.Internal
{
    internal static class Parser
    {
        public static string ParsePlayerVersionHtml(string rawHtml)
        {
            if (rawHtml.IsBlank())
                throw new ArgumentNullException(nameof(rawHtml));

            var match = Regex.Match(rawHtml, @"<script\s*src=""/yts/jsbin/player-(.*?)/base.js", RegexOptions.Multiline);
            if (!match.Success)
                throw new Exception("Could not parse player version");

            return match.Groups[1].Value;
        }

        public static Dictionary<string, string> ParseDictionaryUrlEncoded(string rawUrlEncoded)
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

        public static IEnumerable<VideoStreamInfo> ParseVideoStreamInfosUrlEncoded(string rawUrlEncoded)
        {
            if (rawUrlEncoded.IsBlank())
                throw new ArgumentNullException(nameof(rawUrlEncoded));

            foreach (string streamRaw in rawUrlEncoded.Split(","))
            {
                var dic = ParseDictionaryUrlEncoded(streamRaw);

                // Extract values
                string sig = dic.GetOrDefault("s");
                bool needsDeciphering = sig.IsNotBlank();
                string url = dic.GetOrDefault("url");
                int itag = dic.GetOrDefault("itag").ParseIntOrDefault();
                int width = (dic.GetOrDefault("size")?.SubstringUntil("x")).ParseIntOrDefault();
                int height = (dic.GetOrDefault("size")?.SubstringAfter("x")).ParseIntOrDefault();
                long bitrate = dic.GetOrDefault("bitrate").ParseLongOrDefault();
                double fps = dic.GetOrDefault("fps").ParseDoubleOrDefault();

                // Yield
                yield return new VideoStreamInfo
                {
                    Signature = sig,
                    NeedsDeciphering = needsDeciphering,
                    Url = url,
                    Itag = itag,
                    Resolution = new VideoStreamResolution(width, height),
                    Bitrate = bitrate,
                    Fps = fps
                };
            }
        }

        public static IEnumerable<VideoStreamInfo> ParseVideoStreamInfosMpd(string rawMpd)
        {
            if (rawMpd.IsBlank())
                throw new ArgumentNullException(nameof(rawMpd));

            var root = XElement.Parse(rawMpd);
            var xStreamInfos = root.XPathSelectElements("//*[local-name() = 'Representation']");

            if (xStreamInfos == null)
                throw new Exception("Cannot find streams in input XML");

            foreach (var xStreamInfo in xStreamInfos)
            {
                // Skip partial streams
                string initUrl =
                    xStreamInfo.XPathSelectElement(".//*[local-name() = 'Initialization']")?
                        .Attribute("sourceURL")?.Value;
                if (initUrl.IsNotBlank() && initUrl.ContainsInvariant("sq/"))
                    continue;

                // Extract values
                string url = xStreamInfo.XPathSelectElement("*[local-name() = 'BaseURL']")?.Value;
                int itag = (xStreamInfo.Attribute("id")?.Value).ParseIntOrDefault();
                int width = (xStreamInfo.Attribute("width")?.Value).ParseIntOrDefault();
                int height = (xStreamInfo.Attribute("height")?.Value).ParseIntOrDefault();
                long bitrate = (xStreamInfo.Attribute("bandwidth")?.Value).ParseLongOrDefault();
                double fps = (xStreamInfo.Attribute("frameRate")?.Value).ParseDoubleOrDefault();

                // Yield
                yield return new VideoStreamInfo
                {
                    Signature = null,
                    NeedsDeciphering = false,
                    Url = url,
                    Itag = itag,
                    Resolution = new VideoStreamResolution(width, height),
                    Bitrate = bitrate,
                    Fps = fps
                };
            }
        }

        public static IEnumerable<VideoCaptionTrackInfo> ParseVideoCaptionTrackInfosUrlEncoded(string rawUrlEncoded)
        {
            if (rawUrlEncoded.IsBlank())
                throw new ArgumentNullException(nameof(rawUrlEncoded));

            foreach (string captionRaw in rawUrlEncoded.Split(","))
            {
                var dic = ParseDictionaryUrlEncoded(captionRaw);

                // Extract values
                string url = dic.GetOrDefault("u");
                string lang = dic.GetOrDefault("lc");

                // Yield
                yield return new VideoCaptionTrackInfo
                {
                    Url = url,
                    Language = lang
                };
            }
        }

        public static VideoInfo ParseVideoInfoUrlEncoded(string rawUrlEncoded)
        {
            if (rawUrlEncoded.IsBlank())
                throw new ArgumentNullException(nameof(rawUrlEncoded));

            // Get data
            var videoInfoEncoded = ParseDictionaryUrlEncoded(rawUrlEncoded);

            // Check the status
            string status = videoInfoEncoded.GetOrDefault("status");
            string reason = videoInfoEncoded.GetOrDefault("reason");
            if (status.EqualsInvariant("fail"))
                throw new YoutubeErrorException(reason);

            // Prepare result
            var result = new VideoInfo();

            // Populate data
            result.Id = videoInfoEncoded.GetOrDefault("video_id");
            result.Title = videoInfoEncoded.GetOrDefault("title");
            result.Author = videoInfoEncoded.GetOrDefault("author");
            result.Watermarks = videoInfoEncoded.GetOrDefault("watermark").Split(",");
            result.Length = TimeSpan.FromSeconds(videoInfoEncoded.GetOrDefault("length_seconds").ParseDoubleOrDefault());
            result.IsListed = videoInfoEncoded.GetOrDefault("is_listed").ParseIntOrDefault(1) == 1;
            result.IsRatingAllowed = videoInfoEncoded.GetOrDefault("allow_ratings").ParseIntOrDefault(1) == 1;
            result.IsMuted = videoInfoEncoded.GetOrDefault("muted").ParseIntOrDefault() == 1;
            result.IsEmbeddingAllowed = videoInfoEncoded.GetOrDefault("allow_embed").ParseIntOrDefault(1) == 1;
            result.ViewCount = videoInfoEncoded.GetOrDefault("view_count").ParseLongOrDefault();
            result.AverageRating = videoInfoEncoded.GetOrDefault("avg_rating").ParseDoubleOrDefault();
            result.Keywords = videoInfoEncoded.GetOrDefault("keywords").Split(",");
            result.DashMpdUrl = videoInfoEncoded.GetOrDefault("dashmpd");

            // Get the streams
            var streams = new List<VideoStreamInfo>();
            string streamsRaw = videoInfoEncoded.GetOrDefault("adaptive_fmts");
            if (streamsRaw.IsNotBlank())
                streams.AddRange(ParseVideoStreamInfosUrlEncoded(streamsRaw));
            streamsRaw = videoInfoEncoded.GetOrDefault("url_encoded_fmt_stream_map");
            if (streamsRaw.IsNotBlank())
                streams.AddRange(ParseVideoStreamInfosUrlEncoded(streamsRaw));
            result.Streams = streams.ToArray();

            // Get the captions
            var captions = new List<VideoCaptionTrackInfo>();
            string captionsRaw = videoInfoEncoded.GetOrDefault("caption_tracks");
            if (captionsRaw.IsNotBlank())
                captions.AddRange(ParseVideoCaptionTrackInfosUrlEncoded(captionsRaw));
            result.CaptionTracks = captions.ToArray();

            // Return
            return result;
        }

        public static string GetFunctionCallFromLineJs(string rawJs)
        {
            if (rawJs.IsBlank())
                throw new ArgumentNullException(nameof(rawJs));

            var match = Regex.Match(rawJs, @"\w+\.(\w+)\(");
            return match.Groups[1].Value;
        }

        public static IEnumerable<IScramblingOperation> ParseScramblingOperationsJs(string rawJs)
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