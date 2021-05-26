using Financier.DataAccess.Data;
using System;
using System.Text;

namespace Financier.DataAccess.Utils
{
    public static class Utils
    {
        public static Decimal HUNDRED = new Decimal(100);
        public static String TRANSFER_DELIMITER = " \u00BB ";

        public static string setAmountText(Currency c, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            return amountToString(sb, c, amount, addPlus).ToString();
           
        }

        public static string setAmountText(Currency originalCurrency, long originalAmount, Currency currency, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            amountToString(sb, originalCurrency, originalAmount, addPlus);
            sb.Append(" (");
            amountToString(sb, currency, amount, addPlus);
            sb.Append(")");
            return sb.ToString();
        }

        public static String amountToString(Currency c, long amount)
        {
            return amountToString(c, amount, false);
        }

        public static String amountToString(Currency c, Decimal amount)
        {
            StringBuilder sb = new StringBuilder();
            return amountToString(sb, c, amount, false).ToString();
        }

        public static StringBuilder amountToString(StringBuilder sb, Currency c, long amount)
        {
            return amountToString(sb, c, amount, false);
        }

        public static String amountToString(Currency c, long amount, bool addPlus)
        {
            StringBuilder sb = new StringBuilder();
            return amountToString(sb, c, amount, addPlus).ToString();
        }

        public static StringBuilder amountToString(StringBuilder sb, Currency c, long amount, bool addPlus)
        {
            return amountToString(sb, c, new decimal(amount), addPlus);
        }

        public static StringBuilder amountToString(StringBuilder sb, Currency c, decimal amount, bool addPlus)
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
                c = Currency.EMPTY;
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

        public static String getTransferAmountText(Currency fromCurrency, long fromAmount, Currency toCurrency, long toAmount)
        {
            var sb = new StringBuilder();
            if (sameCurrency(fromCurrency, toCurrency))
            {
                Utils.amountToString(sb, fromCurrency, fromAmount);
            }
            else
            {
                Utils.amountToString(sb, fromCurrency, Math.Abs(fromAmount)).Append(TRANSFER_DELIMITER);
                Utils.amountToString(sb, toCurrency, toAmount);
            }
            return sb.ToString();
        }

        public static Boolean sameCurrency(Currency fromCurrency, Currency toCurrency)
        {
            return fromCurrency.Id == toCurrency.Id;
        }

        public static string setTransferBalanceText(Currency fromCurrency, int? fromBalance, Currency toCurrency, int? toBalance)
        {
            var sb = new StringBuilder();
            Utils.amountToString(sb, fromCurrency, fromBalance ?? 0, false).Append(TRANSFER_DELIMITER);
            Utils.amountToString(sb, toCurrency, toBalance ?? 0, false);
            return sb.ToString();
        }
    }
}
