﻿using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Financier.Common
{
    [ExcludeFromCodeCoverage]
    public static class TransactionTitleUtils
    {
        public static string GenerateTransactionTitle(string payee, string note, string location, int? categoryId, string category, int? toAccount)
        {
            if (categoryId == -1)
            {
                return GenerateTransactionTitleForSplit(payee, note, location);
            }
            else
            {
                return GenerateTransactionTitleForRegular(categoryId, payee, note, location, category, toAccount);
            }
        }

        private static void Append(StringBuilder sb, string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                if (sb.Length > 0)
                {
                    sb.Append(": ");
                }
                sb.Append(s);
            }
        }

        private static string GenerateTransactionTitleForRegular(int? categoryId, string payee, string note,
                    string location, string category, int? toAccount)
        {
            var secondPart = JoinAdditionalFields(payee, note, location);
            if (!string.IsNullOrEmpty(category))
            {
                StringBuilder sb = new StringBuilder();
                var cat = categoryId > 0 ? category : NoCategory(toAccount);
                sb.Append(cat);
                if (!string.IsNullOrEmpty(secondPart))
                {
                    sb.Append(" (").Append(secondPart).Append(')');
                }

                return sb.ToString();
            }
            return secondPart;

            static string NoCategory(int? toAccount)
            {
                return toAccount > 0 ? "Transfer" : "<NO CATEGORY>";
            }
        }

        private static string GenerateTransactionTitleForSplit(string payee, string note, string location)
        {
            StringBuilder sb = new StringBuilder();
            var secondPart = JoinAdditionalFields(sb, note, location);
            if (!string.IsNullOrEmpty(payee))
            {
                if (!string.IsNullOrEmpty(secondPart))
                {
                    return sb.Append('[').Append(payee).Append("...] ").Append(secondPart).ToString();
                }

                return sb.Append('[').Append(payee).Append("...]").ToString();
            }

            if (!string.IsNullOrEmpty(secondPart))
            {
                return sb.Append("[...] ").Append(secondPart).ToString();
            }

            return "[...]";
        }

        private static string JoinAdditionalFields(string payee, string note, string location)
        {
            StringBuilder sb = new StringBuilder();

            Append(sb, payee);
            Append(sb, location);
            Append(sb, note);

            return sb.ToString();
        }
        private static string JoinAdditionalFields(StringBuilder sb, string note, string location)
        {
            sb.Clear();
            Append(sb, location);
            Append(sb, note);
            var secondPart = sb.ToString();
            sb.Clear();
            return secondPart;
        }
    }
}