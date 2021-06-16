using System;
using System.Globalization;
using System.Windows.Data;

namespace Financier.Desktop.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        private const string  format = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        private static DateTime StartDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long timestamp = long.Parse(System.Convert.ToString(value));
            return Convert(timestamp).ToString(format);
        }

        public static DateTime Convert(long timestamp)
        {
            return StartDate.AddMilliseconds(timestamp).ToLocalTime();
        }

        public static long ConvertBack(DateTime timestamp)
        {
            DateTimeOffset dto = new DateTimeOffset(timestamp);
            return dto.ToUnixTimeMilliseconds();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateStr = System.Convert.ToString(value);
            if (!DateTime.TryParseExact(dateStr, format, null, DateTimeStyles.None, out var date))
            {
                date = (DateTime)value;
            }

            return ConvertBack(date);
        }
    }
}
