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
        private const int DescriptionStartIndex = 1;
        private const string CsvHeader = "\"Date and time\",Description,MCC,\"Card currency amount, (UAH)\",\"Operation amount\",\"Operation currency\",\"Exchange rate\",\"Commission, (UAH)\",\"Cashback amount, (UAH)\",Balance";
        private const string DateRegexPattern = @"[0-3][0-9]\.[0-1][0-9]\.[0-9]{4} [0-2][0-9]:[0-5][0-9]";
        private const string DoubleRegexPattern = "[+-]?\\d*\\.?\\d+";

        private readonly Regex dateRegex = new Regex(DateRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly Regex numberRegex = new Regex(DoubleRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        protected override string Header { get => CsvHeader; }

        protected override string ParseTransactionsTable(string pageText)
        {
            StringBuilder sb = new ();

            Match firstMatch = this.dateRegex
                .Matches(pageText)
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
            sb.Append($"\"{words[0]} {words[1]}:00\","); // datetime
            sb.Append($"\"{details.Trim()}\"");
            sb.Append(end);
            return sb.ToString();
        }
    }
}
