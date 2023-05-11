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
        private const string CsvHeader = "\"Date and time\",Description,MCC,\"Card currency amount, (UAH)\",\"Operation amount\",\"Operation currency\",\"Exchange rate\",\"Commission, (UAH)\",\"Cashback amount, (UAH)\",Balance";
        private const string DateRegexPattern = @"[0-3][0-9]\.[0-1][0-9]\.[0-9]{4} [0-2][0-9]:[0-5][0-9]";
        private const string DoubleRegexPattern = @"[+-]?\d*\.?\d+";
        private const string CardNumberRegex = @"\d{4}(\*{4})\d{4}";
        private const string NumbersWithSpacingRegex = @"\s\d{0,3}\s\d{0,3}\,\d{0,2}";

        private readonly Regex dateRegex = new Regex(DateRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));
        private readonly Regex numberRegex = new Regex(DoubleRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(1000));

        protected override string Header { get => CsvHeader; }

        public override string BankTitle => "A-Bank";

        protected override string ParseTransactionsTable(string pageText)
        {
            StringBuilder sb = new ();

            Match firstMatch = this.dateRegex
                .Matches(pageText)
                .Skip(1)    // page header
                .FirstOrDefault();
            Match lastMatch = this.numberRegex
                .Matches(pageText)
                .LastOrDefault();

            if (firstMatch != null && lastMatch != null)
            {
                string tableText = pageText.Substring(firstMatch.Index, lastMatch.Index + lastMatch.Length - firstMatch.Index);

                List<int> tableLines = dateRegex
                    .Matches(tableText)
                    .Select(x => x.Index)
                    .ToList();
                tableLines.Add(tableText.Length);

                for (int j = 1; j < tableLines.Count; j++)
                {
                    int line = tableLines[j];
                    int prevLine = tableLines[j - 1];
                    string lineText = tableText.Substring(prevLine, line - prevLine);
                    string csvText = AddSeparators(lineText);
                    sb.AppendLine(csvText);
                }
            }

            return sb.ToString();
        }

        private static string AddSeparators(string line)
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
                .Replace(",", ".")
                .Trim()
                .Split(Space);

            string end = string.Empty, details = string.Empty;
            int caret = WordsCountAfterDescription;
            for (int i = words.Length - 1; i > DescriptionStartIndex; i--)
            {
                if (caret > 0)
                {
                    end = $",{words[i]}{end}";
                    caret--;
                }
                else
                {
                    details = $"{words[i]} {details}";
                }
            }

            StringBuilder sb = new();
            sb.Append($"\"{date}:00\","); // datetime
            sb.Append($"\"{details.Trim()}\"");
            sb.Append(end);
            return sb.ToString();
        }
    }
}
