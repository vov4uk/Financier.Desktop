using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;

namespace Financier.Common.Converters
{
    /// <summary>
    /// Combines a localized label (values[0]) with an optional dynamic value (values[1]).
    /// Returns "Label (value)" when value is non-empty, or just "Label" when value is null/empty.
    /// Bind values[0] to a LocalizationService indexer so the text reacts to culture changes.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LocalizedFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 0) return string.Empty;
            var first = values[0] as string ?? string.Empty;
            if (values.Length == 1) return first;

            // 2 values: "Label (value)" pattern — returns just label when value is null/empty
            if (values.Length == 2)
            {
                var val = values[1] as string;
                return string.IsNullOrEmpty(val) ? first : $"{first} ({val})";
            }

            // 3+ values: first is a format template, rest are positional args
            var args = new object[values.Length - 1];
            Array.Copy(values, 1, args, 0, args.Length);
            return string.Format(first, args);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
