using System;
using System.Globalization;
using System.Windows.Data;

namespace Financier.Common.Converters
{
    public class CategoryTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var title = (string)values[0];
            var level =  (int)(long)values[1];
            return (title ?? string.Empty).PadLeft((title ?? string.Empty).Length + level, '-');
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
