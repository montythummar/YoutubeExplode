// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplode>
//  File: <Ext.cs>
//  Created By: Alexey Golub
//  Date: 08/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Collections.Generic;

namespace YoutubeExplode
{
    internal static class Ext
    {
        public static Uri ToUri(this string uri)
        {
            return new UriBuilder(uri).Uri;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key,
            TValue defaultValue = default(TValue))
        {
            TValue result;
            if (dic.TryGetValue(key, out result))
                return result;
            return defaultValue;
        }

        public static double ParseDoubleOrDefault(this string str, double defaultValue = default(double))
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            double result;
            if (double.TryParse(str, out result))
                return result;
            return defaultValue;
        }

        public static int ParseIntOrDefault(this string str, int defaultValue = default(int))
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            int result;
            if (int.TryParse(str, out result))
                return result;
            return defaultValue;
        }
    }
}