using System;
using System.Globalization;
using System.Windows.Data;

namespace Financier.Desktop.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        private const string  FORMAT = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        private const string  FORMAT_DAY = "yyyy'-'MM'-'dd";
        private static readonly DateTime StartDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long timestamp = long.Parse(System.Convert.ToString(value) ?? string.Empty);
            var date = Convert(timestamp);
            if (date == date.Date)
            {
                return date.ToString(FORMAT_DAY);
            }
            return date.ToString(FORMAT);
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
            if (!DateTime.TryParseExact(dateStr, FORMAT, null, DateTimeStyles.None, out var date))
            {
                date = (DateTime)value;
            }

            return ConvertBack(date);
        }
    }
}
