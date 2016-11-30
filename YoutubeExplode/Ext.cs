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
        public static string[] Split(this string input, params string[] separators)
        {
            return input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Uri ToUri(this string uri)
        {
            return new UriBuilder(uri).Uri;
        }

        public static Uri ToUri(this string uri, string baseUri)
        {
            return new Uri(ToUri(baseUri), uri);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key,
            TValue defaultValue = default(TValue))
        {
            TValue result;
            if (dic.TryGetValue(key, out result))
                return result;
            return defaultValue;
        }

        public static TConverted GetValueOrDefault<TKey, TValue, TConverted>(this IDictionary<TKey, TValue> dic,
            TKey key, TConverted defaultValue = default(TConverted))
        {
            var result = GetValueOrDefault(dic, key);
            return ConvertOrDefault(result, defaultValue);
        }

        public static T ConvertOrDefault<T>(this object obj, T defaultValue = default(T))
        {
            if (obj == null)
                return defaultValue;

            try
            {
                return (T) Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
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

        public static bool EqualsInvariant(this string str, string other)
        {
            if (str == null)
                return other == null;
            return str.Equals(other, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsInvariant(this string str, string other)
        {
            return str?.IndexOf(other, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}