using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Financier.Common.Localization;
using Financier.Desktop.Helpers.BankHelper.Model;
using Financier.Desktop.Wizards;
using MiniExcelLibs;

namespace Financier.Desktop.Helpers.BankHelper
{
    public class PrivatHelper : IBankHelper
    {
        public string BankTitle => LocalizationService.Instance.privat;

        public IEnumerable<BankTransaction> ParseReport(string filePath)
        {
            List<PrivatRow> abankRows = new List<PrivatRow>();
            var rows = MiniExcel.Query(filePath, useHeaderRow: true, excelType: ExcelType.XLSX, startCell: "A2");

            using (var csvStream = new MemoryStream())
            {
                MiniExcel.SaveAs(csvStream, rows, printHeader: true, excelType: ExcelType.CSV);

                using (var csvReader = new StreamReader(csvStream))
                using (var csv = new CsvReader(csvReader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    ShouldSkipRecord = args => args.Row.Parser.Record!.All(string.IsNullOrWhiteSpace),
                    Delimiter = ","
                }))
                {
                    csvStream.Position = 0;
                    var r = csv.GetRecords<PrivatRow>().ToList();
                    abankRows.AddRange(r);
                }
            }

            var transactions = abankRows.Select(MapperHelper.ToBankTransaction).ToList();
            return transactions;
        }
    }
}
