using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Financier.Desktop.Helpers
{
    public class PumbHelper : BankPdfHelperBase
    {
        private const string DateTimeRegexPattern = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})";
        private const string DateRegexPattern = @"(\s\d{4}-\d{2}-\d{2})";
        private const string CardNumberPattern = @"(\d{8}\*{4}\d{4})";
        private const string AmountPattern = @"([0-9]+\.[0-9]+) (UAH|USD|EUR)";
        private const string ReportDatePattern = "( Дата формування)";

        public override string BankTitle => "ПУМБ";

        protected override string Header => "\"Date and time\",\"Operation amount\",\"Operation currency\",\"Card currency amount, (UAH)\",\"Commission, (UAH)\",Description";

        private static Regex lineStartRegex = new Regex(DateTimeRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
        private static Regex pageEndRegex = new Regex(ReportDatePattern, RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
        private static Regex CardNumberRegex = new Regex(CardNumberPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
        private static Regex DateRegex = new Regex(DateRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
        private static Regex AmountRegex = new Regex(AmountPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));

        protected override string ParseTransactionsTable(string pageText)
        {
            var matches = lineStartRegex.Matches(pageText);
            StringBuilder sb = new StringBuilder();
            if (matches.Any())
            {
                int currentPosition = matches.First().Index;
                foreach (Match match in matches.Skip(1))
                {
                    var line = pageText.Substring(currentPosition, match.Index - currentPosition);
                    currentPosition = match.Index;

                    sb.AppendLine(AddSeparators(line));
                }
                var end = pageEndRegex.Match(pageText);
                sb.AppendLine(AddSeparators(pageText.Substring(currentPosition, end.Index - currentPosition).Trim()));
            }
            return sb.ToString();
        }
        private static string RemoveCardNumber(string line)
        {
            return CardNumberRegex.Replace(line, string.Empty);
        }

        private static string RemovePostingDate(string line)
        {
            return DateRegex.Replace(line, string.Empty);
        }

        private static string AddSeparators(string line)
        {
            line = RemoveCardNumber(line);
            line = RemovePostingDate(line);

            var dateTime = Regex.Match(line, DateTimeRegexPattern).Value;
            line = line.Replace(dateTime, string.Empty);

            var amounts = Regex.Matches(line, AmountPattern);
            var amountInCurrency = amounts[0].Groups[1].Value;
            var amountCurrency = amounts[0].Groups[2].Value;

            var amountInUah = amounts[1].Groups[1].Value;
            var commision = amounts.Last();
            var commissionInUah = commision.Groups[1].Value;
            string sign = string.Empty;
            if (!line.Contains("Надходження", StringComparison.OrdinalIgnoreCase))
            {
                sign = "-";
            }

            return $"\"{dateTime}\",{sign}{amountInCurrency},{amountCurrency},{sign}{amountInUah},{commissionInUah},\"{AmountRegex.Replace(line, string.Empty).Trim()}\"";
        }
    }
}
