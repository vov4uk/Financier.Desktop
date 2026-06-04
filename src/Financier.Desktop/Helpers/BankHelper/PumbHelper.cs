using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Financier.Desktop.Helpers.Model;
using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers.BankHelper
{
    public class PumbHelper : BankPdfHelperBase
    {
        public override string BankTitle => "ПУМБ";

        protected override IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages)
        {
            string pageText = string.Join(Environment.NewLine, pages);


            List<Pumb_Row> converted = new List<Pumb_Row>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ";",
                IgnoreBlankLines = true,
                MissingFieldFound = null,
                ShouldSkipRecord = (e) =>
                {
                    return string.IsNullOrEmpty(e.Row[0]) || e.Row.ColumnCount != 8;
                }
            };

            using (var csv = new CsvReader(new StringReader(pageText), config))
            {
                var records = csv.GetRecords<Pumb_Row>();

                var batch = records.Take(1000).ToList();
                converted.AddRange(batch);
            }


            var transactions = new List<BankTransaction>();

            foreach (var item in converted.Where(x => !string.IsNullOrWhiteSpace(x.OperationAmount)))
            {
                string sign = string.Empty;
                if (!string.Equals(item.TransactionType, "Надходження", StringComparison.OrdinalIgnoreCase))
                {
                    sign = "-";
                }


                var operationAmount = GetDouble(RemoveCurrencyCode(item.OperationAmount));
                var operationCurrency = item.OperationAmount.Replace(RemoveCurrencyCode(item.OperationAmount), "").Trim();
                var cardCurrenyAmount = GetDouble(RemoveCurrencyCode(item.CardCurrenyAmount));
                var commisionAmount = GetDouble(RemoveCurrencyCode(item.CommisionAmount));

                var tr = new BankTransaction
                {
                    Commission = commisionAmount,
                    OperationCurrency = operationCurrency,
                    OperationAmount = GetDouble($"{sign}{operationAmount}"),
                    CardCurrencyAmount = GetDouble($"{sign}{cardCurrenyAmount}"),
                    ExchangeRate = operationAmount == cardCurrenyAmount ? null : Math.Round(cardCurrenyAmount / operationAmount, 4),
                    Description = $"{item.Details} {item.TransactionType}",
                    Date = item.Date
                };
                transactions.Add(tr);
            }

            return transactions.OrderByDescending(x => x.Date).ToList();
        }


        private static string RemoveCurrencyCode(string amount)
        {
            return amount.Replace("UAH", string.Empty)
                .Replace("USD", string.Empty)
                .Replace("EUR", string.Empty)
                .Replace("PLN", string.Empty)
                .Trim();
        }
    }
}
