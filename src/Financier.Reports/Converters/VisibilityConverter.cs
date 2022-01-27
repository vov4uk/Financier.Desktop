using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Financier.Reports.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            if (value != null && (bool)value)
                visibility = Visibility.Visible;
            return visibility;
        }

        public object ConvertBack(
          object value,
          Type targetType,
          object parameter,
          CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }
    }
}