using Financier.Common.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Financier.Converters
{
    [ExcludeFromCodeCoverage]
    public class MccConverter : IValueConverter
    {
        private static Dictionary<int, string> mcc;
        private static Dictionary<int, string> MCC
        {
            get
            {
                return mcc ??= DbManual.MCCCategories.SelectMany(x => x.Value.Select(y => new KeyValuePair<int, string>(y, x.Key))).ToDictionary(x => x.Key, y => y.Value);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int code = (int)value;
            if (MCC.ContainsKey(code))
            {
                  return MCC[code];
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
