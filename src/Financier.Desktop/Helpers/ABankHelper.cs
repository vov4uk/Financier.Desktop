using CsvHelper;
using Docnet.Core;
using Docnet.Core.Models;
using Financier.DataAccess.Monobank;
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
    internal class ABankHelper : IBankHelper
    {
        private const string csvHeader = "\"Date and time\",Description,MCC,\"Card currency amount, (UAH)\",\"Operation amount\",\"Operation currency\",\"Exchange rate\",\"Commission, (UAH)\",\"Cashback amount, (UAH)\",Balance";
        private const string endText = "ПІДПИС БАНКУ";
        private const string startText = @"Залишок
після операціЇ";
        private const string dateRegex = @"[0-3][0-9]\.[0-1][0-9]\.[0-9]{4} [0-2][0-9]:[0-5][0-9]";

        public async Task<IEnumerable<BankTransaction>> ParseReport(string filePath)
        {
            if (File.Exists(filePath))
            {
                StringBuilder sb = new StringBuilder();
                using (var docReader = DocLib.Instance.GetDocReader(filePath, new PageDimensions()))
                {
                    for (var i = 0; i < docReader.GetPageCount(); i++)
                    {
                        using (var pageReader = docReader.GetPageReader(i))
                        {
                            sb.AppendLine(pageReader.GetText());
                        }
                    }
                }

                var table = ParseTransactionsTable(sb.ToString());

                using (TextReader streamReader = new StringReader(table))
                {
                    using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                    {
                        return await csv.GetRecordsAsync<BankTransaction>().ToListAsync();
                    }
                }
            }
            return Array.Empty<BankTransaction>();
        }

        private static string ParseTransactionsTable(string pageText)
        {
            StringBuilder sb = new StringBuilder();
            var tableStartIndex = pageText.IndexOf(startText, StringComparison.InvariantCultureIgnoreCase);
            var tableEndIndex = pageText.IndexOf(endText, StringComparison.InvariantCultureIgnoreCase);

            var tableText = pageText.Substring(tableStartIndex, tableEndIndex - tableStartIndex)
                .Replace(startText, string.Empty)
                .Replace(Environment.NewLine, " ")
                .Trim();

            var regex = new Regex(dateRegex, RegexOptions.Singleline);
            var tableLines = regex.Matches(tableText);

            sb.AppendLine(csvHeader);
            for (int j = 1; j < tableLines.Count; j++)
            {
                var line = tableLines[j];
                var prevLine = tableLines[j - 1];
                var row = tableText.Substring(prevLine.Index, line.Index - prevLine.Index);
                string parsedRow = AddSeparators(row);
                sb.AppendLine(parsedRow);
            }
            var lastLine = tableLines[tableLines.Count - 1];
            var lastRow = tableText.Substring(lastLine.Index);
            string parsedLastRow = AddSeparators(lastRow);
            sb.AppendLine(parsedLastRow);
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
                    details = word + " " + details;
                }
            }

            sb.Append($"\"{details.Trim()}\"");
            sb.Append(end);
            return sb.ToString();
        }
    }
}
