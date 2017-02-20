// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <Decipherer.cs>
//  Created By: Alexey Golub
//  Date: 18/11/2016
// ------------------------------------------------------------------ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    internal partial class Decipherer
    {
        private readonly IScramblingOperation[] _operations;

        public Decipherer(IEnumerable<IScramblingOperation> operations)
        {
            if (operations == null)
                throw new ArgumentNullException(nameof(operations));

            _operations = operations.ToArray();
        }

        private string ApplyAllOperations(string signature)
        {
            if (signature.IsBlank())
                throw new ArgumentNullException(nameof(signature));

            foreach (var op in _operations)
                signature = op.Unscramble(signature);
            return signature;
        }

        public void UnscrambleSignatures(VideoInfo videoInfo)
        {
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (!videoInfo.NeedsDeciphering)
                throw new Exception("Given video info does not need to be deciphered");

            // Return if there aren't any streams
            if (videoInfo.Streams == null || !videoInfo.Streams.Any())
            {
                videoInfo.NeedsDeciphering = false;
                return;
            }

            // Update signatures on streams
            foreach (var stream in videoInfo.Streams.Where(s => s.NeedsDeciphering))
            {
                string sig = stream.Signature;
                string newSig = ApplyAllOperations(sig);

                // Update signature
                stream.Signature = newSig;

                // Update URL
                if (stream.Url.ContainsInvariant("signature"))
                    stream.Url = stream.Url.Replace(sig, newSig);
                else
                    stream.Url += $"&signature={newSig}";

                // Update flag
                stream.NeedsDeciphering = false;
            }

            // Update global flag
            videoInfo.NeedsDeciphering = false;
        }
    }

    internal partial class Decipherer
    {
        private static string GetFunctionCallFromLine(string line)
        {
            if (line.IsBlank())
                throw new ArgumentNullException(nameof(line));

            var match = Regex.Match(line, @"\w+\.(\w+)\(");
            return match.Groups[1].Value;
        }

        private static IEnumerable<IScramblingOperation> GetScramblingOperations(string playerRawJs)
        {
            if (playerRawJs.IsBlank())
                throw new ArgumentNullException(nameof(playerRawJs));

            // Get the name of the function that handles deciphering
            var funcNameMatch = Regex.Match(playerRawJs, @"\""signature"",\s?([a-zA-Z0-9\$]+)\(");
            if (!funcNameMatch.Success)
                throw new Exception("Could not find the entry function for signature deciphering");
            string funcName = funcNameMatch.Groups[1].Value;

            // Escape dollar sign
            funcName = funcName.Replace("$", "\\$");

            // Get the body of the function
            var funcBodyMatch = Regex.Match(playerRawJs, @"(?!h\.)" + funcName + @"=function\(\w+\)\{.*?\}",
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
                string calledFunctionName = GetFunctionCallFromLine(line);

                // Compose regexes to identify what function we're dealing with
                // -- reverse (1 param)
                var reverseFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\(\w+\)");
                // -- slice (return or not)
                var sliceFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.");
                // -- swap
                var swapFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b");

                // Determine the function type and assign the name
                if (reverseFuncRegex.Match(playerRawJs).Success)
                    reverseFuncName = calledFunctionName;
                else if (sliceFuncRegex.Match(playerRawJs).Success)
                    sliceFuncName = calledFunctionName;
                else if (swapFuncRegex.Match(playerRawJs).Success)
                    charSwapFuncName = calledFunctionName;
            }

            // Analyze the function body again to determine the operation set and order
            foreach (var line in funcLines)
            {
                // Get the function called on this line
                string calledFunctionName = GetFunctionCallFromLine(line);

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

        public static Decipherer FromPlayerSource(string playerRawJs)
        {
            if (playerRawJs.IsBlank())
                throw new ArgumentNullException(nameof(playerRawJs));

            return new Decipherer(GetScramblingOperations(playerRawJs));
        }
    }
}