using System;
using System.Globalization;
using System.Windows.Data;
using Financier.Common.Entities;

namespace Financier.Converters
{
    public class MccConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int code = (int)value;
            if (DbManual.MCCCodes.TryGetValue(code, out var mccValue))
            {
                return mccValue;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
