using Financier.Desktop.Wizards;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Financier.Desktop.Helpers
{
    public class PireusHelper : BankPdfHelperBase
    {
        private const int WordsCountAfterDescription = 1;

        private const string DateRegexPattern = @"[0-3][0-9]\.[0-1][0-9]\.[0-9]{4}\r\n\d{1,2}:[0-5][0-9]:[0-5][0-9]";
        private const string DoubleRegexPattern = @"[+-]?\d*\.?\d+";
        private const string CardNumberRegex = @"\d{6}(\*{6})\d{4}";
        private const string NumbersWithSpacingRegex = @"([-|\s])\d{1,3}\s\d{0,3}\.\d{0,2}";

        private readonly Regex dateRegex = new Regex(DateRegexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
        private readonly Regex numberRegex = new Regex(DoubleRegexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));

        public override string BankTitle => "Pireus";

        protected override IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages)
        {
            var transactions = new List<BankTransaction>();

            foreach (var page in pages)
            {
                var dates = this.dateRegex.Matches(page);
                if (dates?.Any() == true)
                {
                    List<int> tableLines = dates
                        .Select(x => x.Index)
                        .ToList();

                    for (int j = 1; j < dates.Count; j++)
                    {
                        int line = tableLines[j];
                        int prevLine = tableLines[j - 1];
                        string lineText = page.Substring(prevLine, line - prevLine);

                        transactions.Add(ParseLine(lineText));
                    }

                    var lastLineStartIndex = tableLines.Last();
                    var lastLine = page.Substring(lastLineStartIndex);
                    var lastLineEndIndex = this.numberRegex.Matches(lastLine).LastOrDefault();
                    if (lastLineEndIndex != null)
                    {
                        lastLine = lastLine.Substring(0, lastLineEndIndex.Index + lastLineEndIndex.Length);
                        transactions.Add(ParseLine(lastLine));
                    }
                }
            }

            return transactions.OrderByDescending(x => x.Date);
        }

        private static BankTransaction ParseLine(string line)
        {
            var date = Regex.Match(line, DateRegexPattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(30)).Value;
            line = line.Replace(date, string.Empty);

            line = line.Replace("\r\n", Space);

            var cardNumber = Regex.Match(line, CardNumberRegex, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(30));
            if (cardNumber.Success)
            {
                line = line.Replace(cardNumber.Value, string.Empty);
            }
            var numbersWithSpaces = Regex.Matches(line, NumbersWithSpacingRegex, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(30));
            foreach (var number in numbersWithSpaces.Select(x => x.Value))
            {
                var numberWithoutSpaces = " " + number.Replace(" ", string.Empty);
                line = line.Replace(number, numberWithoutSpaces);
            }

            var words = line
                .Trim()
                .Split(Space)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            var wordsCount = words.Length;
            var description = new List<string>();
            for (int i = 5; i < words.Length - WordsCountAfterDescription; i++)
            {
                description.Add(words[i]);
            }

            var operationCurrency = words[1];
            var operationAmount = GetDouble(words[0]);
            var cardCurrencyAmount = GetDouble(words[3] == "-" ? words[0] : words[3]);

            return new BankTransaction
            {
                Balance = GetDouble(words[wordsCount - 1]),

                Commission = GetDouble(words[4]),
                OperationCurrency = operationAmount != cardCurrencyAmount ? operationCurrency : null,
                OperationAmount = operationAmount,
                CardCurrencyAmount = cardCurrencyAmount,
                Description = string.Join(Space, description),
                Date = DateTime.Parse(date.Replace("\r\n", Space))
            };
        }
    }
}
