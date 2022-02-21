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
    public class ABankHelper : IBankHelper
    {
        private const string CsvHeader = "\"Date and time\",Description,MCC,\"Card currency amount, (UAH)\",\"Operation amount\",\"Operation currency\",\"Exchange rate\",\"Commission, (UAH)\",\"Cashback amount, (UAH)\",Balance";
        private const string DateRegexPattern = @"[0-3][0-9]\.[0-1][0-9]\.[0-9]{4} [0-2][0-9]:[0-5][0-9]";
        private const string DoubleRegexPattern = "[+-]?\\d*\\.?\\d+";
        private const string Space = " ";

        private readonly Regex dateRegex = new Regex(DateRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly Regex numberRegex = new Regex(DoubleRegexPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        public async Task<IEnumerable<BankTransaction>> ParseReport(string filePath)
        {
            if (File.Exists(filePath))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(CsvHeader);
                using (var docReader = DocLib.Instance.GetDocReader(filePath, new PageDimensions()))
                {
                    for (var i = 0; i < docReader.GetPageCount(); i++)
                    {
                        using (var pageReader = docReader.GetPageReader(i))
                        {
                            var pageText = ParseTransactionsTable(pageReader.GetText().Replace(Environment.NewLine, Space));
                            sb.AppendLine(pageText);
                        }
                    }
                }

                using (TextReader streamReader = new StringReader(sb.ToString()))
                {
                    using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        return await csv.GetRecordsAsync<BankTransaction>().ToListAsync();
                    }
                }
            }
            return Array.Empty<BankTransaction>();
        }

        private string ParseTransactionsTable(string pageText)
        {
            StringBuilder sb = new StringBuilder();

            Match firstMatch = this.dateRegex.Matches(pageText).FirstOrDefault();
            Match lastMatch = this.numberRegex.Matches(pageText).LastOrDefault();

            if (firstMatch != null && lastMatch != null)
            {
                string tableText = pageText.Substring(firstMatch.Index, lastMatch.Index + lastMatch.Length - firstMatch.Index);

                List<int> tableLines = dateRegex.Matches(tableText).Select(x => x.Index).ToList();
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
            var words = line.Replace(",", ".").Trim().Split(' ');

            StringBuilder sb = new StringBuilder();

            sb.Append($"\"{words[0]} {words[1]}:00\",");

            string end = string.Empty;
            int caret = 8;
            string details = string.Empty;
            for (int i = words.Length - 1; i > 1; i--)
            {
                string word = words[i];
                if (caret > 0)
                {
                    end = "," + word + end;
                    caret--;
                }
                else
                {
                    details = word + Space + details;
                }
            }

            sb.Append($"\"{details.Trim()}\"");
            sb.Append(end);
            return sb.ToString();
        }
    }
}
