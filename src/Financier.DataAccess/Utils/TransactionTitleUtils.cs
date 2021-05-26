using System.Text;

namespace Financier.DataAccess
{
    public static class TransactionTitleUtils
    {
        public static string GenerateTransactionTitle(string payee, string note, string location, int? categoryId, string category)
        {
            if (categoryId == -1) {
                return generateTransactionTitleForSplit(payee, note, location);
            } else 
            {
                return generateTransactionTitleForRegular(categoryId, payee, note, location, category);
            }
        }

        private static string generateTransactionTitleForRegular(int? categoryId, string payee, string note, string location, string category)
        {
            StringBuilder sb = new StringBuilder();
            var secondPart = joinAdditionalFields(sb, payee, note, location);
            if (!string.IsNullOrEmpty(category))
            {
                var cat = categoryId > 0 ? category : categoryId == 0 ? "Transfer" : "[...]";
                sb.Append(cat);
                if (!string.IsNullOrEmpty(secondPart))
                {
                    sb.Append(" (").Append(secondPart).Append(")");
                }
                return sb.ToString();
            }
            else
            {
                return secondPart;
            }
        }

        private static string joinAdditionalFields(StringBuilder sb, string payee, string note, string location)
        {
            sb.Clear();
            Append(sb, payee);
            Append(sb, location);
            Append(sb, note);
            var secondPart = sb.ToString();
            sb.Clear();
            return secondPart;
        }

        private static string generateTransactionTitleForSplit(string payee, string note, string location)
        {
            StringBuilder sb = new StringBuilder();
            var secondPart = joinAdditionalFields(sb, note, location);
            if (!string.IsNullOrEmpty(payee))
            {
                if (!string.IsNullOrEmpty(secondPart))
                {
                    return sb.Append("[").Append(payee).Append("...] ").Append(secondPart).ToString();
                }
                else { return sb.Append("[").Append(payee).Append("...]").ToString(); }
            }
            else
            {
                if (!string.IsNullOrEmpty(secondPart))
                {
                    return sb.Append("[...] ").Append(secondPart).ToString();
                }
                else return "[...]";
            }
        }

        private static string joinAdditionalFields(StringBuilder sb, string note, string location)
        {
            sb.Clear();
            Append(sb, location);
            Append(sb, note);
            var secondPart = sb.ToString();
            sb.Clear();
            return secondPart;
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
    }
}
