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
using System.Text;
using System.Text.RegularExpressions;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    internal static class Decipherer
    {
        private static string GetFunctionCallFromLine(string line)
        {
            var match = Regex.Match(line, @"\w+\.(\w+)\(", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            return match.Groups[1].Value;
        }

        private static IEnumerable<ScramblingOperation> GetOperations(string playerRawJs)
        {
            // Get the name of the function that handles deciphering
            var funcNameMatch = Regex.Match(playerRawJs, @"\.sig\s*\|\|([a-zA-Z0-9\$]+)\(",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            if (!funcNameMatch.Success)
                throw new Exception("Could not find the entry function for signature deciphering");
            string funcName = funcNameMatch.Groups[1].Value;

            // Escape dollar sign
            funcName = funcName.Replace("$", "\\$");

            // Get the body of the function
            var funcBodyMatch = Regex.Match(playerRawJs, @"(?!h\.)" + funcName + @"=function\(\w+\)\{.*?\}",
                RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            if (!funcBodyMatch.Success)
                throw new Exception("Could not get the signature decipherer function body");
            string funcBody = funcBodyMatch.Value;
            var funcLines = funcBody.Split(";");

            // Identify scrambling functions
            string reverseFuncName = null;
            string sliceFuncName = null;
            string charSwapFuncName = null;

            // Analyze the function body (skip first and last line) to determine the names of scrambling functions
            foreach (var line in funcLines.Skip(1).Take(funcLines.Length - 2))
            {
                // Break when all functions are found
                if (!reverseFuncName.IsBlank() && !sliceFuncName.IsBlank() && !charSwapFuncName.IsBlank())
                    break;

                // Get the function called on this line
                string calledFunctionName = GetFunctionCallFromLine(line);

                // Compose regexes to identify what function we're dealing with
                // -- reverse (1 param)
                var reverseFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\(\w+\)",
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                // -- slice (return or not)
                var sliceFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.",
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                // -- swap
                var swapFuncRegex = new Regex($@"{calledFunctionName}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b",
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                // Determine the function type and assign the name
                if (reverseFuncRegex.Match(playerRawJs).Success)
                    reverseFuncName = calledFunctionName;
                else if (sliceFuncRegex.Match(playerRawJs).Success)
                    sliceFuncName = calledFunctionName;
                else if (swapFuncRegex.Match(playerRawJs).Success)
                    charSwapFuncName = calledFunctionName;
            }

            // Make sure all are set
            if (reverseFuncName.IsBlank() || sliceFuncName.IsBlank() || charSwapFuncName.IsBlank())
                throw new Exception("Could not determine the name of one or more scrambling functions");

            // Analyze the function body again to determine the operation set and order
            foreach (var line in funcLines.Skip(1).Take(funcLines.Length - 2))
            {
                // Get the function called on this line
                string calledFunctionName = GetFunctionCallFromLine(line);

                // Swap operation
                if (calledFunctionName.EqualsInvariant(charSwapFuncName))
                {
                    int index = Regex.Match(line, @"\(\w+,(\d+)\)").Groups[1].Value.ParseIntOrDefault();
                    yield return new ScramblingOperation(ScramblingOperationType.Swap, index);
                }
                // Slice operation
                else if (calledFunctionName.EqualsInvariant(sliceFuncName))
                {
                    int index = Regex.Match(line, @"\(\w+,(\d+)\)").Groups[1].Value.ParseIntOrDefault();
                    yield return new ScramblingOperation(ScramblingOperationType.Slice, index);
                }
                // Reverse operation
                else if (calledFunctionName.EqualsInvariant(reverseFuncName))
                {
                    yield return new ScramblingOperation(ScramblingOperationType.Reverse);
                }
            }
        }

        private static string ApplyOperation(ScramblingOperation operation, string signature)
        {
            if (signature.IsBlank())
                throw new ArgumentNullException(signature);

            switch (operation.Type)
            {
                // Swap first char of the signature with one of given index
                case ScramblingOperationType.Swap:
                {
                    var sb = new StringBuilder(signature)
                    {
                        [0] = signature[operation.Parameter],
                        [operation.Parameter] = signature[0]
                    };
                    return sb.ToString();
                }
                // Substring past the given index
                case ScramblingOperationType.Slice:
                    return signature.Substring(operation.Parameter);
                // Reverse string
                case ScramblingOperationType.Reverse:
                    return signature.Reverse();
                // Sanity check
                default:
                    throw new NotImplementedException("Non-implemented scrambling operation");
            }
        }

        private static string ApplyOperations(IEnumerable<ScramblingOperation> operations, string signature)
        {
            if (signature.IsBlank())
                throw new ArgumentNullException(signature);

            foreach (var operation in operations)
                signature = ApplyOperation(operation, signature);

            return signature;
        }

        public static void UnscrambleSignatures(VideoInfo videoInfo, string playerRawJs)
        {
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (playerRawJs.IsBlank())
                throw new ArgumentNullException(nameof(playerRawJs));

            // No streams => nothing to decipher => we're good
            if (videoInfo.Streams == null || !videoInfo.Streams.Any())
            {
                videoInfo.NeedsDeciphering = false;
                return;
            }

            // Get operations
            var operations = GetOperations(playerRawJs).ToArray();

            // Update signatures on videostreams
            foreach (var stream in videoInfo.Streams.Where(s => s.NeedsDeciphering))
            {
                string sig = stream.Signature;
                string newSig = ApplyOperations(operations, sig);

                // Update signature
                stream.Signature = newSig;

                // Update URL
                if (stream.URL.ContainsInvariant("signature"))
                    stream.URL = stream.URL.Replace(sig, newSig);
                else
                    stream.URL += $"&signature={newSig}";

                // Update flag
                stream.NeedsDeciphering = false;
            }

            // Update global flag
            videoInfo.NeedsDeciphering = false;
        }
    }
}
