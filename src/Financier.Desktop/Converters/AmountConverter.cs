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
                if (System.Convert.ToString(parameter) == "false")
                {
                    return amount / 100.0m;
                }
                return Math.Abs(amount / 100.0m);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var amount = System.Convert.ToDecimal(value);
            return System.Convert.ToInt64(amount * 100.0m);
        }
    }
}