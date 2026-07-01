using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Financier.Common.Entities;
using Financier.Common.Localization;
using Financier.Common.Model;
using Financier.DataAccess.Data;

namespace Financier.Common.Utils
{
    [ExcludeFromCodeCoverage]
    public static class BlotterUtils
    {
        public const string TRANSFER_DELIMITER = " \u00BB ";
        internal const decimal HUNDRED = 100m;
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
                return LocalizationService.Instance[$"account_type_{type}".ToLowerInvariant()];
            }
            return sb.ToString();
        }

        public static string GetTransferAmountText(CurrencyModel fromCurrency, long fromAmount, CurrencyModel toCurrency, long toAmount)
        {
            var sb = new StringBuilder();
            if (fromCurrency.Id == toCurrency.Id)
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
                currency = new CurrencyModel(Currency.EMPTY);
            }

            string s = (amount / HUNDRED).ToString("N2", currency.getFormat()).TrimEnd('.');

            sb.Append(s);
            if (!string.IsNullOrEmpty(currency.Symbol))
            {
                if (Enum.TryParse<SymbolFormat>(currency.SymbolFormat, out var symbolFormat))
                {
                    symbolFormat.AppendSymbol(sb, currency.Symbol);
                }
                else
                {
                    sb.Append(' ').Append(currency.Symbol);
                }
            }

            return sb;
        }
    }
}
