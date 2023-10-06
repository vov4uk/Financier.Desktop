using CsvHelper;
using Docnet.Core;
using Docnet.Core.Models;
using Financier.Desktop.Wizards;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Financier.Desktop.Helpers
{
    public class ABankHelper : BankPdfHelperBase
    {
        private const int WordsCountAfterDescription = 8;
        private const int DescriptionStartIndex = -1;

        private const string DateRegexPattern = @"[0-3][0-9]\.[0-1][0-9]\.[0-9]{4}\r\n[0-2][0-9]:[0-5][0-9]";
        private const string DoubleRegexPattern = @"[+-]?\d*\.?\d+";
        private const string CardNumberRegex = @"\d{4}(\*{4})\d{4}";
        private const string NumbersWithSpacingRegex = @"([-|\s])\d{1,3}\s\d{0,3}\,\d{0,2}";

        private readonly Regex dateRegex = new Regex(DateRegexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
        private readonly Regex numberRegex = new Regex(DoubleRegexPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));

        public override string BankTitle => "A-Bank";

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
            return transactions;
        }

        private static BankTransaction ParseLine(string line)
        {
            var date = Regex.Match(line, DateRegexPattern).Value;
            line = line.Replace(date, string.Empty);
            var cardNumber = Regex.Match(line, CardNumberRegex);
            if (cardNumber.Success)
            {
                line = line.Replace(cardNumber.Value, string.Empty);
            }
            var numbersWithSpaces = Regex.Matches(line, NumbersWithSpacingRegex);
            foreach (Match number in numbersWithSpaces)
            {
                var numberWithoutSpaces = " " + number.Value.Replace(" ", string.Empty);
                line = line.Replace(number.Value, numberWithoutSpaces);
            }

            var words = line
                .Trim()
                .Split(Space)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            var wordsCount = words.Length;
            var desctiption = new List<string>();
            for (int i = 0; i < words.Length - WordsCountAfterDescription; i++)
            {
                desctiption.Add(words[i]);
            }

            var operationCurrency = words[wordsCount - 5];
            var operationAmount = GetDouble(words[wordsCount - 6]);
            var cardCurrencyAmount = GetDouble(words[wordsCount - 7]);

            return new BankTransaction
            {
                Balance = GetDouble(words[wordsCount - 1]),
                Cashback = GetDouble(words[wordsCount - 2]),
                Commission = GetDouble(words[wordsCount - 3]),
                ExchangeRate = GetDouble(words[wordsCount - 4] == "-" ? "0" : words[wordsCount - 4]),
                OperationCurrency = operationAmount != cardCurrencyAmount ? operationCurrency : null,
                OperationAmount = operationAmount,
                CardCurrencyAmount = cardCurrencyAmount,
                MCC = words[wordsCount - 8],
                Description = string.Join(Space, desctiption),
                Date = Convert.ToDateTime(date.Replace("\r\n", Space))
            };
        }
    }
}
