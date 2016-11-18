// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <Decipherer.cs>
//  Created By: Alexey Golub
//  Date: 18/11/2016
// ------------------------------------------------------------------ 

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YoutubeExplode.Models;

namespace YoutubeExplode
{
    // Stolen from https://github.com/flagbug/YoutubeExtractor/blob/master/YoutubeExtractor/YoutubeExtractor/Decipherer.cs
    // TODO: refactor this piece of shit

    internal static class Decipherer
    {
        private static string GetOperations(string rawJs)
        {
            //Find "C" in this: var A = B.sig||C (B.s)
            string functNamePattern = @"\.sig\s*\|\|([a-zA-Z0-9\$]+)\("; //Regex Formed To Find Word or DollarSign

            var funcName = Regex.Match(rawJs, functNamePattern).Groups[1].Value;

            if (funcName.Contains("$"))
            {
                funcName = "\\" + funcName; //Due To Dollar Sign Introduction, Need To Escape
            }

            string funcPattern = @"(?!h\.)" + funcName + @"=function\(\w+\)\{.*?\}"; //Escape funcName string
            var funcBody = Regex.Match(rawJs, funcPattern, RegexOptions.Singleline).Value; //Entire sig function
            var lines = funcBody.Split(';'); //Each line in sig function

            string idReverse = "", idSlice = "", idCharSwap = ""; //Hold name for each cipher method
            string functionIdentifier;
            string operations = "";

            foreach (var line in lines.Skip(1).Take(lines.Length - 2)) //Matches the funcBody with each cipher method. Only runs till all three are defined.
            {
                if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                    !string.IsNullOrEmpty(idCharSwap))
                {
                    break; //Break loop if all three cipher methods are defined
                }

                functionIdentifier = GetFunctionFromLine(line);
                string reReverse = $@"{functionIdentifier}:\bfunction\b\(\w+\)"; //Regex for reverse (one parameter)
                string reSlice = $@"{functionIdentifier}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\."; //Regex for slice (return or not)
                string reSwap = $@"{functionIdentifier}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b"; //Regex for the char swap.

                if (Regex.Match(rawJs, reReverse).Success)
                {
                    idReverse = functionIdentifier; //If def matched the regex for reverse then the current function is a defined as the reverse
                }

                if (Regex.Match(rawJs, reSlice).Success)
                {
                    idSlice = functionIdentifier; //If def matched the regex for slice then the current function is defined as the slice.
                }

                if (Regex.Match(rawJs, reSwap).Success)
                {
                    idCharSwap = functionIdentifier; //If def matched the regex for charSwap then the current function is defined as swap.
                }
            }

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                Match m;
                functionIdentifier = GetFunctionFromLine(line);

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                {
                    operations += "w" + m.Groups["index"].Value + " "; //operation is a swap (w)
                }

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                {
                    operations += "s" + m.Groups["index"].Value + " "; //operation is a slice
                }

                if (functionIdentifier == idReverse) //No regex required for reverse (reverse method has no parameters)
                {
                    operations += "r "; //operation is a reverse
                }
            }

            return operations.Trim();
        }

        private static string ApplyOperation(string cipher, string op)
        {
            switch (op[0])
            {
                case 'r':
                    return new string(cipher.ToCharArray().Reverse().ToArray());

                case 'w':
                    {
                        int index = GetOpIndex(op);
                        return SwapFirstChar(cipher, index);
                    }

                case 's':
                    {
                        int index = GetOpIndex(op);
                        return cipher.Substring(index);
                    }

                default:
                    throw new NotImplementedException("Couldn't find cipher operation.");
            }
        }

        private static string DecipherWithOperations(string cipher, string operations)
        {
            return operations.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(cipher, ApplyOperation);
        }

        private static string GetFunctionFromLine(string currentLine)
        {
            var matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\("); //lc.ac(b,c) want the ac part.
            var rgMatch = matchFunctionReg.Match(currentLine);
            string matchedFunction = rgMatch.Groups["functionID"].Value;
            return matchedFunction; //return 'ac'
        }

        private static int GetOpIndex(string op)
        {
            string parsed = new Regex(@".(\d+)").Match(op).Result("$1");
            int index = int.Parse(parsed);

            return index;
        }

        private static string SwapFirstChar(string cipher, int index)
        {
            var builder = new StringBuilder(cipher)
            {
                [0] = cipher[index],
                [index] = cipher[0]
            };

            return builder.ToString();
        }

        public static void Decipher(VideoInfo videoInfo, string rawJs)
        {
            if (videoInfo == null)
                throw new ArgumentNullException(nameof(videoInfo));
            if (string.IsNullOrWhiteSpace(rawJs))
                throw new ArgumentNullException(nameof(rawJs));

            // No streams => nothing to decipher => we're good
            if (videoInfo.Streams == null || !videoInfo.Streams.Any())
            {
                videoInfo.NeedsDeciphering = false;
                return;
            }

            // Get operations
            string operations = GetOperations(rawJs);

            // Update signatures on videostreams
            foreach (var stream in videoInfo.Streams)
            {
                string sig = stream.Signature;
                string newSig = DecipherWithOperations(sig, operations);

                // Update signature
                stream.Signature = newSig;

                // Update URL
                if (stream.URL.ContainsInvariant("signature"))
                    stream.URL = stream.URL.Replace(sig, newSig);
                else
                    stream.URL += $"&signature={newSig}";
            }

            videoInfo.NeedsDeciphering = false;
        }
    }
}
