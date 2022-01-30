using Financier.DataAccess.Data;
using System;
using System.Globalization;
using System.Text;

namespace Financier.DataAccess.Utils
{
    public static class Utils
    {
        internal const decimal HUNDRED = 100m;
        internal const string TRANSFER_DELIMITER = " \u00BB ";

        public static string SetAmountText(Currency c, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            return AmountToString(sb, c, amount, addPlus).ToString();
           
        }

        internal static string SetAmountText(Currency originalCurrency, long originalAmount, Currency currency, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            AmountToString(sb, originalCurrency, originalAmount, addPlus);
            sb.Append(" (");
            AmountToString(sb, currency, amount, addPlus);
            sb.Append(")");
            return sb.ToString();
        }

        private static StringBuilder AmountToString(StringBuilder sb, Currency currency, long amount)
        {
            return AmountToString(sb, currency, amount, false);
        }

        private static StringBuilder AmountToString(StringBuilder sb, Currency currency, long amount, bool addPlus)
        {
            return AmountToString(sb, currency, new decimal(amount), addPlus);
        }

        private static StringBuilder AmountToString(StringBuilder sb, Currency currency, decimal amount, bool addPlus)
        {
            if (amount.CompareTo(decimal.Zero) > 0 && addPlus)
            {
                sb.Append("+");
            }
            if (currency == null)
            {
                currency = Currency.Empty;
            }
            string s = (amount / HUNDRED).ToString("F2", CultureInfo.InvariantCulture);
            if (s.EndsWith("."))
            {
                s = s.Substring(0, s.Length - 1);
            }
            sb.Append(s);
            if (!string.IsNullOrEmpty(currency.Symbol))
            {
              sb.Append(" ").Append(currency.Symbol);
            }
            return sb;
        }

        internal static string GetTransferAmountText(Currency fromCurrency, long fromAmount, Currency toCurrency, long toAmount)
        {
            var sb = new StringBuilder();
            if (SameCurrency(fromCurrency, toCurrency))
            {
                Utils.AmountToString(sb, fromCurrency, fromAmount);
            }
            else
            {
                Utils.AmountToString(sb, fromCurrency, Math.Abs(fromAmount)).Append(TRANSFER_DELIMITER);
                Utils.AmountToString(sb, toCurrency, toAmount);
            }
            return sb.ToString();
        }

        private static bool SameCurrency(Currency fromCurrency, Currency toCurrency)
        {
            return fromCurrency.Id == toCurrency.Id;
        }

        internal static string SetTransferBalanceText(Currency fromCurrency, int? fromBalance, Currency toCurrency, int? toBalance)
        {
            var sb = new StringBuilder();
            Utils.AmountToString(sb, fromCurrency, fromBalance ?? 0, false).Append(TRANSFER_DELIMITER);
            Utils.AmountToString(sb, toCurrency, toBalance ?? 0, false);
            return sb.ToString();
        }
    }
}
