using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace fcrd
{
    public class InverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)(value == null ? 2 : (!(bool)value ? 0 : 2));

        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            return ((Visibility)value != Visibility.Visible);
        }
    }
}