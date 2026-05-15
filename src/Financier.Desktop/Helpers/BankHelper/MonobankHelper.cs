using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers.BankHelper
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MonobankHelper : IBankHelper
    {
        public string BankTitle => "Monobank";

        public IEnumerable<BankTransaction> ParseReport(string filePath)
        {
            if (File.Exists(filePath))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    MissingFieldFound = null,
                    HasHeaderRecord = true,
                    ShouldSkipRecord = (e) =>
                    {
                        return e.Row.Parser.RawRecord.Contains("www.fg.gov.ua");
                    }
                };

                using FileStream file = File.OpenRead(filePath);
                using StreamReader streamReader = new StreamReader(file, Encoding.UTF8);
                using (var csv = new CsvReader(streamReader, config))
                {
                    return csv.GetRecords<BankTransaction>().ToList();
                }
            }
            return Array.Empty<BankTransaction>();
        }
    }
}
