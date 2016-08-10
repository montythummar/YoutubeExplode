// ------------------------------------------------------------------ 
//  Solution: <YoutubeExplode>
//  Project: <YoutubeExplodeDemo>
//  File: <EnumToStringConverter.cs>
//  Created By: Alexey Golub
//  Date: 10/08/2016
// ------------------------------------------------------------------ 

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace YoutubeExplodeDemo.WPF
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumObj = (Enum) value;
            string str = enumObj.ToString();
            return Regex.Replace(str, @"([a-z])([A-Z])", @"$1 $2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}