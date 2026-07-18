using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Financier.Converters
{
    [ExcludeFromCodeCoverage]
    public class AccountTypeConverter : IMultiValueConverter
    {
        private static HashSet<string> KnownTypes = new HashSet<string> { "asset", "bank", "cash", "electronic", "liability" };
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            string type = null;
            string card_issuer = null;

            if (values.Length > 0)
                type = values[0]?.ToString()?.ToLowerInvariant();
            if (values.Length > 1)
                card_issuer = values[1]?.ToString()?.ToLowerInvariant();

            return new BitmapImage(GetImageUrl(type, card_issuer));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static Uri GetImageUrl(string type, string card_issuer)
        {
            if (!string.IsNullOrEmpty(type) && type.Contains("card") && !string.IsNullOrEmpty(card_issuer))
            {
                return new Uri($"pack://application:,,,/Images/AccountType/account_type_card_{card_issuer}.png");
            }
            if (!string.IsNullOrEmpty(type) && KnownTypes.Contains(type))
            {
                return new Uri($"pack://application:,,,/Images/AccountType/account_type_{type}.png");
            }
            return new Uri("pack://application:,,,/Images/AccountType/account_type_other.png");
        }
    }
}
