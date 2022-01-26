using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Financier.Reports.Converters
{
    public class InverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)(value == null ? 2 : (bool)value ? 2 : 0);
        }

        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            return (Visibility)value != Visibility.Visible;
        }
    }
}