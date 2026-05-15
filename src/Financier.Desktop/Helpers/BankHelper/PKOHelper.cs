using CsvHelper;
using Financier.Desktop.Wizards;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tabula;
using Tabula.Extractors;
using UglyToad.PdfPig;

namespace Financier.Desktop.Helpers.BankHelper
{
    public class PKOHelper : IBankHelper
    {
        public string BankTitle => "PKO";

        public IEnumerable<BankTransaction> ParseReport(string filePath)
        {
            List<BankTransaction> result = new();
            if (File.Exists(filePath))
            {
                List<string> pages = new List<string>();
                List<PKO_Transaction> transactions = new List<PKO_Transaction>();

                using (PdfDocument document = PdfDocument.Open(filePath, new ParsingOptions() { ClipPaths = true }))
                {
                    for (int i = 0; i < document.NumberOfPages; i++)
                    {
                        PageArea page = ObjectExtractor.Extract(document, i + 1);

                        IExtractionAlgorithm ea = new BasicExtractionAlgorithm();

                        var pageTables = ea.Extract(page);
                        if (!pageTables.Any())
                        {
                            continue;
                        }

                        Table table = pageTables.OrderBy(x => x.Cells.Count).Last();


                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (IReadOnlyList<Cell> row in table.Rows)
                        {

                            var allEmpty = row.All(x => string.IsNullOrEmpty(x.GetText()));
                            if (!allEmpty)
                            {
                                foreach (Cell item in row)
                                {
                                    var text = item.GetText().Replace("\r", " ").Trim();
                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        stringBuilder.Append(text + "|");
                                    }
                                }
                            }
                        }

                        pages.Add(stringBuilder.ToString());
                    }
                }

                foreach (string page in pages)
                {
                    var startTextIndex = page.IndexOf("Data waluty Opis operacji|");
                    var pageText = page.Substring(startTextIndex + 26);
                    var endTextIndex = Math.Max(pageText.IndexOf("|Saldo do przeniesienia"), pageText.IndexOf("|Saldo końcowe"));
                    if (endTextIndex < 0)
                    {
                        continue;
                    }
                    pageText = pageText.Substring(0, endTextIndex);

                    // Pattern to match: date (DD.MM.YYYY) followed by space and ID (4 letters + 11 digits)
                    var operationDatePattern = @"([0-9]{2}\.[0-9]{2}\.[0-9]{4}) ([0-9]{4}[A-Z]{2}[0-9]{11})";
                    var rowStart = Regex.Matches(pageText, operationDatePattern);

                    for (int i = 0; i < rowStart.Count; i++)
                    {
                        var match = rowStart[i];
                        var dateStr = match.Groups[1].Value;
                        var idStr = match.Groups[2].Value;

                        // Get the substring from current match to the next match (or end of string)
                        int blockStart = match.Index;
                        int blockEnd = (i + 1 < rowStart.Count) ? rowStart[i + 1].Index : pageText.Length;
                        var transactionBlock = pageText.Substring(blockStart + match.Length, blockEnd - blockStart - match.Length).TrimEnd('|');

                        // Remove spaces between numbers (e.g., "3 200,00" -> "3200,00")
                        transactionBlock = Regex.Replace(transactionBlock, @"(?<=[0-9])\s+(?=[0-9])", "").Trim();

                        var decimalPattern = @"-?\d+(?:,\d+)";

                        var decimalMatches = Regex.Matches(transactionBlock, decimalPattern);

                        var operationAmount = decimalMatches[0];
                        var saldo = decimalMatches[1];
                        var transactionType = transactionBlock.Substring(0, operationAmount.Index).Trim('|').Trim();

                        var transactionDescriprion = transactionBlock.Substring(saldo.Index + saldo.Length).Trim('|').Trim();

                        var dataWaluty = Regex.Match(transactionDescriprion, @"([0-9]{2}\.[0-9]{2}\.[0-9]{4})");
                        if (dataWaluty.Success)
                        {
                            transactionDescriprion = transactionDescriprion.Substring(dataWaluty.Length);
                        }

                        var dataWalutyGodzina = Regex.Match(transactionDescriprion, @"Godz\.([0-9]{2}:[0-9]{2}:[0-9]{2})");
                        string dataWalutyGodzinaStr = "00:00:00";
                        if (dataWalutyGodzina.Success)
                        {
                            transactionDescriprion = transactionDescriprion.Replace(dataWalutyGodzina.Value, "");
                            dataWalutyGodzinaStr = dataWalutyGodzina.Groups[1].Value;
                        }

                        // Parse the transaction data
                        DateTime.TryParse(dateStr, CultureInfo.CurrentCulture, DateTimeStyles.None, out var transactionDate);
                        DateTime.TryParse($"{dataWaluty.Value} {dataWalutyGodzinaStr}", CultureInfo.CurrentCulture, DateTimeStyles.None, out var currencyDate);

                        var kwotaOryg = Regex.Match(transactionDescriprion, @"(Kwota oryg\.:) (-?\d+(?:,\d+)) ([A-Z]{3})");
                        if (kwotaOryg.Success)
                        {
                            transactionDescriprion = transactionDescriprion.Replace(kwotaOryg.Value, "");
                        }

                        transactionDescriprion = transactionDescriprion.Replace("|", "").Trim();
                        transactionDescriprion = Regex.Replace(transactionDescriprion, @"(Nr ref:|Nrref:)( |)([0-9]{17})", string.Empty).Trim();
                        transactionDescriprion = Regex.Replace(transactionDescriprion, @"Tel\.:[0-9]{11}", string.Empty).Trim();
                        transactionDescriprion = Regex.Replace(transactionDescriprion, @"Karta:[0-9]{6}\*\*\*\*\*\*[0-9]{4}", string.Empty).Trim();
                        transactionDescriprion = Regex.Replace(transactionDescriprion, @"([0-9]{26})", " ").Trim();
                        transactionDescriprion = transactionDescriprion.Replace("Lokalizacja:", string.Empty).Trim();

                        var transaction = new PKO_Transaction
                        {
                            DataOperacji = transactionDate,
                            IdentyfikatorOperacji = idStr,
                            DataWaluty = currencyDate,
                            KwotaOperacji = BankPdfHelperBase.GetDouble(operationAmount.Value),
                            Saldo = BankPdfHelperBase.GetDouble(saldo.Value),
                            OpisOperacji = transactionDescriprion,
                            TypOperacji = transactionType,
                            KwotaOryg = kwotaOryg.Success ? BankPdfHelperBase.GetDouble(kwotaOryg.Groups[2].Value) : BankPdfHelperBase.GetDouble(operationAmount.Value),
                            WalutaOryg = kwotaOryg.Success ? kwotaOryg.Groups[3].Value : string.Empty
                        };
                        transactions.Add(transaction);
                    }
                }

                foreach (var transaction in transactions)
                {
                    result.Add(new BankTransaction
                    {
                        Date = transaction.DataWaluty,
                        Description = $"{transaction.TypOperacji}\r\n{transaction.OpisOperacji}",
                        OperationAmount = transaction.KwotaOperacji,
                        Balance = transaction.Saldo,
                        OperationCurrency = transaction.WalutaOryg,
                        CardCurrencyAmount = (transaction.KwotaOperacji < 0 && transaction.KwotaOperacji != transaction.KwotaOryg) ?  -1* transaction.KwotaOryg : transaction.KwotaOryg,
                        MCC = null
                    });
                }
            }
            return result;
        }

        private class PKO_Transaction
        {
            public DateTime DataOperacji { get; set; }
            public string IdentyfikatorOperacji { get; set; }
            public string TypOperacji { get; set; }
            public double KwotaOperacji { get; set; }
            public double Saldo { get; set; }
            public DateTime DataWaluty { get; set; }
            public string OpisOperacji { get; set; }
            public double KwotaOryg { get; set; }
            public string WalutaOryg { get; set; }
        }
    }
}
