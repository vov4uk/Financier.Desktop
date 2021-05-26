using System;
using System.Globalization;
using System.Windows.Data;

namespace Financier.Desktop.Converters
{
    public class AmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (long.TryParse(System.Convert.ToString(value), out var amount))
            {
                return (amount / 100.0).ToString("F2");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var amount = System.Convert.ToDouble(value);
            return (long)(amount *100);
        }
    }
}
