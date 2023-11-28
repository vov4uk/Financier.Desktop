using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
            var type = ((string)values[0])?.ToLowerInvariant();
            var card_issuer = ((string)values[1])?.ToLowerInvariant();


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
