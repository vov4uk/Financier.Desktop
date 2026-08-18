using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using Financier.Common.Entities;
using Financier.Common.Model;

namespace Financier.Converters
{
    public class DifferentCurrencyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 4
                || values[0] is not int fromAccountId
                || values[1] is not int toAccountId
                || values[2] is not int categoryId
                || values[3] is not AccountFilterModel monoAccount)
            {
                return false;
            }

            if (categoryId > 0)
            {
                return false;
            }

            var selectedAccount = DbManual.Account.FirstOrDefault(a => fromAccountId > 0 && a.Id == fromAccountId)
                                   ?? DbManual.Account.FirstOrDefault(a => toAccountId > 0 && a.Id == toAccountId);

            if (selectedAccount == null)
            {
                return false;
            }

            return selectedAccount.CurrencyId != monoAccount.CurrencyId;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
