using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Financier.Converters
{
    public class OnlyOneSelectedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Count(v => v is int intValue && intValue > 0) == 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
