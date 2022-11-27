using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Docnet.Core.Models;
using Docnet.Core;
using Financier.Desktop.Wizards;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Financier.Desktop.Helpers
{
    public class RaiffeisenHelper : BankPdfHelperBase
    {
        private const int WordsCountAfterDescription = 4;
        private const int DescriptionStartIndex = 5;
        private const string CsvHeader = "\"Date and time\",Description,\"Card currency amount, (UAH)\",\"Operation amount\",\"Operation currency\",Balance";
        private const string DateRegexPattern = @"(([0-3][0-9]\.[0-1][0-9]\.[0-9]{4})\/  ([0-3][0-9]\.[0-1][0-9]\.[0-9]{4}))";
        private const string CurrencyRegexPattern = "(UAH|USD|EUR) [0-9| ]+\\,[0-9]+";

        private static Regex lineStartRegex = new Regex(DateRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static Regex lineEndRegex = new Regex(CurrencyRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        protected override string Header => CsvHeader;

        protected override string ParseTransactionsTable(string pageText)
        {
            StringBuilder sb = new ();

            Match firstMatch = lineStartRegex
                .Matches(pageText)
                .FirstOrDefault();
            Match lastMatch = lineEndRegex
                .Matches(pageText)
                .LastOrDefault();

            if (firstMatch != null && lastMatch != null)
            {
                string tableText = pageText.Substring(firstMatch.Index, lastMatch.Index + lastMatch.Length - firstMatch.Index);

                List<int> tableLines = lineStartRegex
                    .Matches(tableText)
                    .Select(x => x.Index)
                    .ToList();
                tableLines.Add(tableText.Length);

                for (int j = 1; j < tableLines.Count; j++)
                {
                    int line = tableLines[j];
                    int prevLine = tableLines[j - 1];
                    string lineText = tableText.Substring(prevLine, line - prevLine);
                    string result = Regex.Replace(lineText, @"\s{1}([+-]?\d{1,3})\s(\d{3}\,\d{2})", @" $1$2");
                    string csvText = AddSeparators(result);
                    sb.AppendLine(csvText);
                }
            }

            return sb.ToString();
        }

        private static string AddSeparators(string line)
        {
            var words = line
                .Replace(",", ".")
                .Replace("/", string.Empty)
                .Replace("торговельна", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Replace("точка", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                .Trim()
                .Split(Space)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            string end = string.Empty, details = string.Empty;
            int caret = WordsCountAfterDescription;
            for (int i = words.Count - 1; i > DescriptionStartIndex; i--) // start from end of line, and take last "WordsCountAfterDescription" words ("Card currency amount, (UAH)","Operation amount","Operation currency",Balance")
            {                                                             // words in middle - became "Description"
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
            sb.Append($"\"{words[0]} 00:00:00\",");
            sb.Append($"\"{details.Trim()}\"");
            sb.Append(end);
            return sb.ToString();
        }
    }
}
