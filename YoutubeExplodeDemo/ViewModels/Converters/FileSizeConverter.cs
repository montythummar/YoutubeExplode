// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <FileSizeConverter.cs>
//  Created By: Alexey Golub
//  Date: 23/01/2017
// ------------------------------------------------------------------ 

using System;
using System.Globalization;
using System.Windows.Data;

namespace YoutubeExplodeDemo.ViewModels.Converters
{
    [ValueConversion(typeof(ulong), typeof(string))]
    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            decimal size = (ulong) value;
            var unit = 0;

            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }

            return $"{size:0.#} {units[unit]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
