using CsvHelper;
using Financier.Desktop.Helpers.Model;
using Financier.Desktop.Wizards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Financier.Desktop.Helpers
{
    public class ABankHelper : BankPdfHelperBase
    {
        public override string BankTitle => "A-Bank";

        protected override IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages)
        {
            List<Abank_Row> abankRows = new List<Abank_Row>();

            foreach (var page in pages)
            {
                using (var csv = new CsvReader(new StringReader(page), DefaultCsvReaderConfig))
                {
                    var records = csv.GetRecords<Abank_Row>().ToList();
                    abankRows.AddRange(records);
                }
            }

            var transactions = abankRows.Select(MapperHelper.ToBankTransaction).ToList();
            return transactions;
        }

        private double ToDouble(string val)
        {
            return BankPdfHelperBase.GetDouble(val.Replace(Space, string.Empty));
        }

    }
}
