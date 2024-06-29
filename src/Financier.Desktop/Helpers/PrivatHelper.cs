using CsvHelper;
using CsvHelper.Configuration;
using Financier.Desktop.Helpers.Model;
using Financier.Desktop.Wizards;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.Helpers
{
    public class PrivatHelper : IBankHelper
    {
        public string BankTitle => "Приват";

        public IEnumerable<BankTransaction> ParseReport(string filePath)
        {
            List<Privat_Row> abankRows = new List<Privat_Row>();
            var rows = MiniExcel.Query(filePath, useHeaderRow: true, excelType: ExcelType.XLSX, startCell: "A2");

            using (var csvStream = new MemoryStream())
            {
                MiniExcel.SaveAs(csvStream, rows, printHeader: true, excelType: ExcelType.CSV);

                using (var csvReader = new StreamReader(csvStream))
                using (var csv = new CsvReader(csvReader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    ShouldSkipRecord = args => args.Row.Parser.Record.All(string.IsNullOrWhiteSpace),
                    Delimiter = ","
                }))
                {
                    csvStream.Position = 0;
                    var r = csv.GetRecords<Privat_Row>().ToList();
                    abankRows.AddRange(r);
                }
            }

            var transactions = abankRows.Select(MapperHelper.ToBankTransaction).ToList();
            return transactions;
        }
    }
}
