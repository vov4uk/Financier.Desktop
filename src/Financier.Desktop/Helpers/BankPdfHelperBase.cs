using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Financier.Desktop.Wizards;
using CsvHelper.Configuration;
using Tabula;
using UglyToad.PdfPig;
using Tabula.Extractors;

namespace Financier.Desktop.Helpers
{
    public abstract class BankPdfHelperBase : IBankHelper
    {
        protected const string Space = " ";

        protected readonly CsvConfiguration DefaultCsvReaderConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            IgnoreBlankLines = true,
            MissingFieldFound = null
        };


        public abstract string BankTitle { get; }

        public IEnumerable<BankTransaction> ParseReport(string filePath)
        {
            if (File.Exists(filePath))
            {
                List<string> pages = new List<string>();

                using (PdfDocument document = PdfDocument.Open(filePath, new ParsingOptions() { ClipPaths = true }))
                {
                    ObjectExtractor oe = new ObjectExtractor(document);
                    for (int i = 0; i < document.NumberOfPages; i++)
                    {
                        PageArea page = oe.Extract(i + 1);

                        IExtractionAlgorithm ea = new SpreadsheetExtractionAlgorithm();

                        var pageTables = ea.Extract(page);
                        if (!pageTables.Any())
                        {
                            continue;
                        }

                        Table table = pageTables.OrderBy(x => x.Cells.Count).Last();

                        using (var stream = new MemoryStream())
                        using (var sb = new StreamWriter(stream) { AutoFlush = true })
                        {
                            CsvWriter csvWriter = new CsvWriter(sb, DefaultCsvReaderConfig);

                            foreach (IReadOnlyList<Cell> row in table.Rows)
                            {

                                var allEmpty = row.All(x => string.IsNullOrEmpty(x.GetText()));
                                if (!allEmpty)
                                {
                                    foreach (Cell item in row)
                                    {
                                        csvWriter.WriteField(item.GetText().Replace("\r", " "));
                                    }

                                    csvWriter.NextRecord();
                                }
                            }

                            var reader = new StreamReader(stream);
                            stream.Position = 0;


                            var data = reader.ReadToEnd().Trim();

                            pages.Add(data);
                        }
                    }
                }

                return ParseTransactionsTable(pages);

            }
            return Array.Empty<BankTransaction>();
        }

        protected abstract IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages);

        protected static double GetDouble(string text)
        {
            double.TryParse(Convert.ToString(text).Replace(',','.'), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out double retNum);
            return retNum;
        }
    }
}
