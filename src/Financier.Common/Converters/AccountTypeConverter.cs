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
                type = (values[0] as string)?.ToLowerInvariant();
            if (values.Length > 1)
                card_issuer = (values[1] as string)?.ToLowerInvariant();

#pragma warning disable CS8604 // Possible null reference argument.
            return new BitmapImage(new Uri(GetImageUri(type, card_issuer)));
#pragma warning restore CS8604 // Possible null reference argument.

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string GetImageUri(string type, string card_issuer)
        {
            if (!string.IsNullOrEmpty(type) && type.Contains("card") && !string.IsNullOrEmpty(card_issuer))
            {
                return $"pack://application:,,,/Images/AccountType/account_type_card_{card_issuer}.png";
            }
            else if (!string.IsNullOrEmpty(type) && KnownTypes.Contains(type))
            {
                return $"pack://application:,,,/Images/AccountType/account_type_{type}.png";
            }
            return "pack://application:,,,/Images/AccountType/account_type_other.png";
        }
    }
}
