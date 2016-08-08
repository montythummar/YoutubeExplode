using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace YoutubeExplodeDemo.WPF
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Hidden;
            var flag = false;

            if (value is bool) flag = (bool)value;
            if (parameter is Visibility) visibility = (Visibility)parameter;

            return (flag ? Visibility.Visible : visibility);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Hidden;

            if (value is Visibility) visibility = (Visibility)value;

            return visibility == Visibility.Visible;
        }
    }
}