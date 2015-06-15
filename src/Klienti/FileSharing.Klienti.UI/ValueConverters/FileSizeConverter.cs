using System;
using System.Globalization;
using System.Windows.Data;

namespace FileSharing.Klienti.UI.ValueConverters
{
    public class FileSizeConverter : IValueConverter
    {
        private static readonly string[] SizeSuffixes = { "bajta", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        private static string SizeSuffix(int value)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));

            return string.Format("{0:n1} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SizeSuffix((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
