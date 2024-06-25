using CsvHelper;
using Financier.Desktop.Helpers.Model;
using Financier.Desktop.Wizards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Financier.Desktop.Helpers
{
    public class PireusHelper : BankPdfHelperBase
    {
        public override string BankTitle => "Pireus";

        protected override IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages)
        {
            var transactions = new List<BankTransaction>();


            List<Pireus_Row> converted = new List<Pireus_Row>();

            using (var csv = new CsvReader(new StringReader(string.Join(Environment.NewLine, pages)), DefaultCsvReaderConfig))
            {
                var records = csv.GetRecords<Pireus_Row>().ToList();
                converted.AddRange(records);
            }

            foreach (var item in converted)
            {
                var operationCurrency = item.OperationCurrency;
                var operationAmount = GetDouble(item.OperationAmount);
                var cardCurrencyAmount = GetDouble(item.CardCurrencyAmount);

                if (cardCurrencyAmount == 0)
                {
                    cardCurrencyAmount = operationAmount;
                }

                if (operationAmount == 0)
                {
                    continue;
                }

                var bt = new BankTransaction
                {
                    Balance = GetDouble(item.Balance),

                    Commission = GetDouble(item.Commision),
                    OperationCurrency = operationAmount != cardCurrencyAmount ? operationCurrency : null,
                    OperationAmount = operationAmount,
                    CardCurrencyAmount = cardCurrencyAmount,
                    Description = item.Details.Replace("(", Space).Replace(")", Space),
                    Date = Convert.ToDateTime(item.Date)
                };

                transactions.Add(bt);
            }

            return transactions.OrderByDescending(x => x.Date);
        }
    }
}
