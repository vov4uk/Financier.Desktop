using Financier.DataAccess.Data;
using System;
using System.Text;

namespace Financier.DataAccess.Utils
{
    public static class Utils
    {
        public static decimal HUNDRED = new Decimal(100);
        public static string TRANSFER_DELIMITER = " \u00BB ";

        public static string SetAmountText(Currency c, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            return AmountToString(sb, c, amount, addPlus).ToString();
           
        }

        public static string SetAmountText(Currency originalCurrency, long originalAmount, Currency currency, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            AmountToString(sb, originalCurrency, originalAmount, addPlus);
            sb.Append(" (");
            AmountToString(sb, currency, amount, addPlus);
            sb.Append(")");
            return sb.ToString();
        }

        public static String AmountToString(Currency c, long amount)
        {
            return AmountToString(c, amount, false);
        }

        public static String AmountToString(Currency c, decimal amount)
        {
            StringBuilder sb = new StringBuilder();
            return AmountToString(sb, c, amount, false).ToString();
        }

        public static StringBuilder AmountToString(StringBuilder sb, Currency c, long amount)
        {
            return AmountToString(sb, c, amount, false);
        }

        public static string AmountToString(Currency c, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            return AmountToString(sb, c, amount, addPlus).ToString();
        }

        public static StringBuilder AmountToString(StringBuilder sb, Currency c, long amount, bool addPlus)
        {
            return AmountToString(sb, c, new decimal(amount), addPlus);
        }

        public static StringBuilder AmountToString(StringBuilder sb, Currency c, decimal amount, bool addPlus)
        {
            if (amount.CompareTo(decimal.Zero) > 0)
            {
                if (addPlus)
                {
                    sb.Append("+");
                }
            }
            if (c == null)
            {
                c = Currency.Empty;
            }
            String s = (amount / HUNDRED).ToString("F2");
            if (s.EndsWith("."))
            {
                s = s.Substring(0, s.Length - 1);
            }
            sb.Append(s);
            if (!string.IsNullOrEmpty(c.Symbol))
            {
              sb.Append(" ").Append(c.Symbol);
            }
            return sb;
        }

        public static String GetTransferAmountText(Currency fromCurrency, long fromAmount, Currency toCurrency, long toAmount)
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

        public static bool SameCurrency(Currency fromCurrency, Currency toCurrency)
        {
            return fromCurrency.Id == toCurrency.Id;
        }

        public static string SetTransferBalanceText(Currency fromCurrency, int? fromBalance, Currency toCurrency, int? toBalance)
        {
            var sb = new StringBuilder();
            Utils.AmountToString(sb, fromCurrency, fromBalance ?? 0, false).Append(TRANSFER_DELIMITER);
            Utils.AmountToString(sb, toCurrency, toBalance ?? 0, false);
            return sb.ToString();
        }
    }
}
