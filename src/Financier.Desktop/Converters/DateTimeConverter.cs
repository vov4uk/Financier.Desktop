using System;
using System.Globalization;
using System.Windows.Data;

namespace Financier.Desktop.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        private const string  format = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
        private DateTime StartDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double timestamp = double.Parse(System.Convert.ToString(value));
            return StartDate.AddMilliseconds(timestamp).ToLocalTime().ToString(format);
        }

        public DateTime Convert(long timestamp)
        {
            return StartDate.AddMilliseconds(timestamp).ToLocalTime();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateStr = System.Convert.ToString(value);
            if (!DateTime.TryParseExact(dateStr, format, null, DateTimeStyles.None, out var date))
            {
                date = (DateTime)value;
            }

            DateTimeOffset dto = new DateTimeOffset(date);
            return dto.ToUnixTimeMilliseconds();
        }
    }
}
