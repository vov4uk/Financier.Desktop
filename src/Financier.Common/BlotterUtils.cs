using Financier.Common.Model;
using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Financier.Common.Utils
{
    [ExcludeFromCodeCoverage]
    public static class BlotterUtils
    {
        public const string TRANSFER_DELIMITER = " \u00BB ";
        internal const decimal HUNDRED = 100m;
        public static string GetTransferAmountText(CurrencyModel fromCurrency, long fromAmount, CurrencyModel toCurrency, long toAmount)
        {
            var sb = new StringBuilder();
            if (SameCurrency(fromCurrency, toCurrency))
            {
                AmountToString(sb, fromCurrency, fromAmount);
            }
            else
            {
                AmountToString(sb, fromCurrency, Math.Abs(fromAmount)).Append(TRANSFER_DELIMITER);
                AmountToString(sb, toCurrency, toAmount);
            }
            return sb.ToString();
        }

        public static string SetAmountText(CurrencyModel c, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            return AmountToString(sb, c, amount, addPlus).ToString();
        }

        public static string SetTransferBalanceText(CurrencyModel fromCurrency, int? fromBalance, CurrencyModel toCurrency, int? toBalance)
        {
            var sb = new StringBuilder();
            AmountToString(sb, fromCurrency, fromBalance ?? 0, false).Append(TRANSFER_DELIMITER);
            AmountToString(sb, toCurrency, toBalance ?? 0, false);
            return sb.ToString();
        }

        public static string GetAccountDescription(string issuer, string number, string type)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(issuer))
            {
                sb.Append(issuer);
            }
            if (!string.IsNullOrEmpty(number))
            {
                sb.Append(" #").Append(number);
            }
            if (sb.Length == 0)
            {
                sb.Append(Char.ToUpperInvariant(type[0]) + type.Substring(1).ToLowerInvariant());
            }
            return sb.ToString();
        }

        internal static string SetAmountText(CurrencyModel originalCurrency, long originalAmount, CurrencyModel currency, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            AmountToString(sb, originalCurrency, originalAmount, addPlus);
            sb.Append(" (");
            AmountToString(sb, currency, amount, addPlus);
            sb.Append(')');
            return sb.ToString();
        }

        private static StringBuilder AmountToString(StringBuilder sb, CurrencyModel currency, long amount)
        {
            return AmountToString(sb, currency, amount, false);
        }

        private static StringBuilder AmountToString(StringBuilder sb, CurrencyModel currency, long amount, bool addPlus)
        {
            return AmountToString(sb, currency, new decimal(amount), addPlus);
        }

        private static StringBuilder AmountToString(StringBuilder sb, CurrencyModel currency, decimal amount, bool addPlus)
        {
            if (amount.CompareTo(decimal.Zero) > 0 && addPlus)
            {
                sb.Append('+');
            }
            if (currency == null)
            {
                currency = new CurrencyModel
                {
                    Id = 0,
                    Name = "",
                    Symbol = ""
                };
            }
            string s = (amount / HUNDRED).ToString("F2", CultureInfo.InvariantCulture);
            if (s.EndsWith('.'))
            {
                s = s.Substring(0, s.Length - 1);
            }
            sb.Append(s);
            if (!string.IsNullOrEmpty(currency.Symbol))
            {
                sb.Append(' ').Append(currency.Symbol);
            }
            return sb;
        }
        private static bool SameCurrency(CurrencyModel fromCurrency, CurrencyModel toCurrency)
        {
            return fromCurrency.Id == toCurrency.Id;
        }
    }
}
