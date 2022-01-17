using System;
using System.Globalization;
using System.Windows.Data;

namespace fcrd
{
    public class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = false;
            if (value != null && value is bool)
                flag = !(bool)value;
            return flag;
        }

        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }
}