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
    }
}