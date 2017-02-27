using System;
using System.Collections.Generic;
using System.Linq;

namespace YoutubeExplode
{
    internal static class Ext
    {
        public delegate T ParseDelegate<out T>(string str);

        public delegate bool TryParseDelegate<T>(string str, out T result);

        public static bool IsBlank(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotBlank(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static string Reverse(this string str)
        {
            return new string(str.Reverse<char>().ToArray());
        }

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

        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key,
            TValue defaultValue = default(TValue))
        {
            TValue result;
            if (dic.TryGetValue(key, out result))
                return result;
            return defaultValue;
        }

        public static TConverted GetOrDefault<TKey, TValue, TConverted>(this IDictionary<TKey, TValue> dic,
            TKey key, TConverted defaultValue = default(TConverted))
        {
            var result = GetOrDefault(dic, key);
            return ConvertOrDefault(result, defaultValue);
        }

        public static T ConvertOrDefault<T>(this object obj, T defaultValue = default(T))
        {
            if (obj == null && typeof(T).IsValueType)
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

        public static T ParseOrDefault<T>(this string str, TryParseDelegate<T> handler, T defaultValue = default(T))
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            T result;
            return handler(str, out result) ? result : defaultValue;
        }

        public static double ParseDoubleOrDefault(this string str, double defaultValue = default(double))
            => ParseOrDefault(str, double.TryParse, defaultValue);

        public static int ParseIntOrDefault(this string str, int defaultValue = default(int))
            => ParseOrDefault(str, int.TryParse, defaultValue);

        public static ulong ParseUlongOrDefault(this string str, ulong defaultValue = default(ulong))
            => ParseOrDefault(str, ulong.TryParse, defaultValue);

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
