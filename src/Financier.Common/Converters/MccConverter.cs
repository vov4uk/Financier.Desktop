using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Financier.Common.Entities;

namespace Financier.Converters
{
    public class MccConverter : IValueConverter
    {
        private static Dictionary<int, Mcc> mccenum = default!;
        private static Dictionary<int, Mcc> MCCenum
        {
            get
            {
                return mccenum ??= DbManual.MCCEnums.SelectMany(x => x.Value.Select(y => new KeyValuePair<int, Mcc>(y, x.Key))).ToDictionary(x => x.Key, y => y.Value);
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int code = (int)value;
            if (MCCenum.ContainsKey(code))
            {
                  return MCCenum[code];
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
