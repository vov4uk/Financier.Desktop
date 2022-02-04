using Financier.DataAccess.Data;
using Financier.DataAccess.Utils;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Financier.Converters
{
    public class AmountTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return "N/A";

            if (long.TryParse(values[0].ToString(), out var totalAmount))
            {
                return Utils.SetAmountText(values[1] as Currency, totalAmount, false);
            }

            return values[0].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
