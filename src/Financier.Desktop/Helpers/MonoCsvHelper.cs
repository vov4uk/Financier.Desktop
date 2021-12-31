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
    public interface ICsvHelper
    {
        Task<IEnumerable<MonoTransaction>> ParseCsv(string csvFilePath);
    }

    public class MonoCsvHelper : ICsvHelper
    {
        public async Task<IEnumerable<MonoTransaction>> ParseCsv(string csvFilePath)
        {
            if (File.Exists(csvFilePath))
            {
                await using FileStream file = File.OpenRead(csvFilePath);
                using StreamReader streamReader = new StreamReader(file, Encoding.UTF8);
                using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    return await csv.GetRecordsAsync<MonoTransaction>().ToListAsync();
                }
            }
            return Array.Empty<MonoTransaction>();
        }
    }
}
