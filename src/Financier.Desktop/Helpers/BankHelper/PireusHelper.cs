using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Financier.Common.Localization;
using Financier.Desktop.Helpers.BankHelper.Model;
using Financier.Desktop.Wizards;
using static Financier.Common.Utils.DoubleUtils;

namespace Financier.Desktop.Helpers.BankHelper
{
    public class PireusHelper : BankPdfHelperBase
    {
        public override string BankTitle => LocalizationService.Instance.pireus;

        protected override IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages)
        {
            var transactions = new List<BankTransaction>();


            List<PireusRow> converted = new List<PireusRow>();

            using (var csv = new CsvReader(new StringReader(string.Join(Environment.NewLine, pages)), DefaultCsvReaderConfig))
            {
                var records = csv.GetRecords<PireusRow>().ToList();
                converted.AddRange(records);
            }

            foreach (var item in converted)
            {
                var operationCurrency = item.OperationCurrency;
                var operationAmount = GetDouble(item.OperationAmount);
                var cardCurrencyAmount = GetDouble(item.CardCurrencyAmount);

                if (DoubleEqual(cardCurrencyAmount, 0))
                {
                    cardCurrencyAmount = operationAmount;
                }

                if (DoubleEqual(operationAmount, 0))
                {
                    continue;
                }

                var bt = new BankTransaction
                {
                    Balance = GetDouble(item.Balance),

                    Commission = GetDouble(item.Commision),
                    OperationCurrency = DoubleNotEqual(operationAmount, cardCurrencyAmount) ? operationCurrency! : null!,
                    OperationAmount = operationAmount,
                    CardCurrencyAmount = cardCurrencyAmount,
                    Description = item.Details.Replace("(", Space).Replace(")", Space),
                    Date = MapperHelper.ParseDateTime(item.Date)
                };

                transactions.Add(bt);
            }

            return transactions.OrderByDescending(x => x.Date);
        }
    }
}
