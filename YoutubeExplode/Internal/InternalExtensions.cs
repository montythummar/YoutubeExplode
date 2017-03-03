using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace YoutubeExplode.Internal
{
    internal static class InternalExtensions
    {
        public delegate T ParseDelegate<out T>(string str);

        public delegate bool TryParseDelegate<T>(string str, out T result);

        public static T ConvertOrDefault<T>(this object obj, T defaultValue = default(T))
        {
            try
            {
                return (T) Convert.ChangeType(obj, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool IsEither<T>(this T value, params T[] potentialValues)
        {
            foreach (var o in potentialValues)
            {
                if (Equals(value, o)) return true;
            }
            return false;
        }

        public static bool IsInRange(this IComparable value, IComparable min, IComparable max)
        {
            return value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
        }

        public static bool IsBlank(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotBlank(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static bool EqualsInvariant(this string str, string other)
        {
            if (str == null)
                return other == null;
            return str.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsInvariant(this string str, string other)
        {
            return str.IndexOf(other, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string SubstringUntil(this string str, string sub)
        {
            int index = str.IndexOf(sub, StringComparison.OrdinalIgnoreCase);
            if (index < 0) return str;
            return str.Substring(0, index);
        }

        public static string SubstringAfter(this string str, string sub)
        {
            int index = str.IndexOf(sub, StringComparison.OrdinalIgnoreCase);
            if (index < 0) return string.Empty;
            return str.Substring(index + sub.Length, str.Length - index - sub.Length);
        }

        public static T Parse<T>(this string str, ParseDelegate<T> handler)
        {
            return handler(str);
        }

        public static T ParseOrDefault<T>(this string str, TryParseDelegate<T> handler, T defaultValue = default(T))
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            T result;
            return handler(str, out result) ? result : defaultValue;
        }

        public static double ParseDouble(this string str)
            => Parse(str, double.Parse);

        public static int ParseInt(this string str)
            => Parse(str, int.Parse);

        public static uint ParseUint(this string str)
            => Parse(str, uint.Parse);

        public static long ParseLong(this string str)
            => Parse(str, long.Parse);

        public static ulong ParseUlong(this string str)
            => Parse(str, ulong.Parse);

        public static double ParseDoubleOrDefault(this string str, double defaultValue = default(double))
            => ParseOrDefault(str, double.TryParse, defaultValue);

        public static int ParseIntOrDefault(this string str, int defaultValue = default(int))
            => ParseOrDefault(str, int.TryParse, defaultValue);

        public static uint ParseUintOrDefault(this string str, uint defaultValue = default(uint))
            => ParseOrDefault(str, uint.TryParse, defaultValue);

        public static long ParseLongOrDefault(this string str, long defaultValue = default(long))
            => ParseOrDefault(str, long.TryParse, defaultValue);

        public static ulong ParseUlongOrDefault(this string str, ulong defaultValue = default(ulong))
            => ParseOrDefault(str, ulong.TryParse, defaultValue);

        public static string Reverse(this string str)
        {
            var sb = new StringBuilder(str.Length);
            for (int i = str.Length - 1; i >= 0; i--)
                sb.Append(str[i]);
            return sb.ToString();
        }

        public static string UrlEncode(this string url)
        {
            return WebUtility.UrlEncode(url);
        }

        public static string UrlDecode(this string url)
        {
            return WebUtility.UrlDecode(url);
        }

        public static string SetQueryStringParameter(this string queryString, string key, string value)
        {
            // Parameter already present
            var existingMatch = Regex.Match(queryString, $@"[?&]{key}=(.+?)(?:&|$)");
            if (existingMatch.Success)
            {
                string existingValue = existingMatch.Groups[1].Value;
                return queryString.Replace(existingValue, value);
            }

            // Not yet present
            bool hasOtherParams = queryString.IndexOf('?') >= 0;
            string separator = hasOtherParams ? "&" : "?";
            return queryString + separator + key + "=" + value;
        }

        public static string JoinToString<T>(this IEnumerable<T> enumerable, string separator)
        {
            return string.Join(separator, enumerable);
        }

        public static string[] Split(this string input, params string[] separators)
        {
            return input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<T> With<T>(this IEnumerable<T> e1, IEnumerable<T> e2)
        {
            var list = e1.ToList();
            list.AddRange(e2);
            return list;
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> enumerable, int count)
        {
            if (count == 0)
                return Enumerable.Empty<T>();

            return enumerable.Reverse().Take(count).Reverse();
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> enumerable, int count)
        {
            if (count == 0)
                return enumerable;

            return enumerable.Reverse().Skip(count).Reverse();
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
            if (result == null) return defaultValue;
            return ConvertOrDefault(result, defaultValue);
        }
    }
}