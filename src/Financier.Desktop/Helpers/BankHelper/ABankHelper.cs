using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Financier.Common.Localization;
using Financier.Desktop.Helpers.BankHelper.Model;
using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers.BankHelper
{
    public class ABankHelper : BankPdfHelperBase
    {
        public override string BankTitle => LocalizationService.Instance.a_bank;

        protected override IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages)
        {
            List<AbankRow> abankRows = new List<AbankRow>();

            foreach (var page in pages)
            {
                using (var csv = new CsvReader(new StringReader(page), DefaultCsvReaderConfig))
                {
                    var records = csv.GetRecords<AbankRow>().ToList();
                    abankRows.AddRange(records);
                }
            }

            var transactions = abankRows.Select(MapperHelper.ToBankTransaction).ToList();
            return transactions;
        }
    }
}
