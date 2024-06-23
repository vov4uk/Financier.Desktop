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
            List<Abank_Row> converted = new List<Abank_Row>();

            foreach (var page in pages)
            {
                using (var csv = new CsvReader(new StringReader(page), DefaultCsvReaderConfig))
                {
                    var records = csv.GetRecords<Abank_Row>();

                    var batch = records.Take(1000).ToList();
                    converted.AddRange(batch);
                }
            }

            var transactions = new List<BankTransaction>();

            foreach (var item in converted)
            {

                var operationCurrency = item.OperationCurrency;
                var operationAmount = ToDouble(item.OperationAmount);
                var cardCurrencyAmount = ToDouble(item.CardCurrencyAmount);

                var bt = new BankTransaction
                {
                    Balance = ToDouble(item.Balance),
                    Cashback = ToDouble(item.Cashback),
                    Commission = ToDouble(item.Commision),
                    ExchangeRate = ToDouble(item.ExchangeRate),
                    OperationCurrency = operationAmount != cardCurrencyAmount ? operationCurrency : null,
                    OperationAmount = operationAmount,
                    CardCurrencyAmount = cardCurrencyAmount,
                    MCC = item.MCC,
                    Description = item.Details,
                    Date = item.Date
                };
                transactions.Add(bt);
            }

            return transactions;
        }

        private double ToDouble(string val)
        {
            return BankPdfHelperBase.GetDouble(val.Replace(Space, string.Empty));
        }

    }
}
