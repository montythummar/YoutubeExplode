using System;
using System.Globalization;
using System.Windows.Data;
using YoutubeExplode.Models;

namespace YoutubeExplode.DemoWpf.ViewModels.Converters
{
    [ValueConversion(typeof(VideoStreamQuality), typeof(string))]
    public class VideoStreamQualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var q = (VideoStreamQuality) value;
            if (q == VideoStreamQuality.Low144) return "144p";
            if (q == VideoStreamQuality.Low240) return "240p";
            if (q == VideoStreamQuality.Medium360) return "360p";
            if (q == VideoStreamQuality.Medium480) return "480p";
            if (q == VideoStreamQuality.High720) return "720p";
            if (q == VideoStreamQuality.High1080) return "1080p";
            if (q == VideoStreamQuality.High1440) return "1440p";
            if (q == VideoStreamQuality.High2160) return "2160p";
            if (q == VideoStreamQuality.High3072) return "3072p";
            return "???";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}