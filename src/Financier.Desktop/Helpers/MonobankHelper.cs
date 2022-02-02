using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Financier.DataAccess.Monobank;

namespace Financier.Desktop.Helpers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class MonobankHelper : IBankHelper
    {
        public async Task<IEnumerable<BankTransaction>> ParseReport(string filePath)
        {
            if (File.Exists(filePath))
            {
                await using FileStream file = File.OpenRead(filePath);
                using StreamReader streamReader = new StreamReader(file, Encoding.UTF8);
                using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    return await csv.GetRecordsAsync<BankTransaction>().ToListAsync();
                }
            }
            return Array.Empty<BankTransaction>();
        }
    }
}
