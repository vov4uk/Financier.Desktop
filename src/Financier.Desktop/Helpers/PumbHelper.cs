using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Financier.Desktop.Wizards;
using Newtonsoft.Json;

namespace Financier.Desktop.Helpers
{
    public class PumbHelper : BankPdfHelperBase
    {
        public override string BankTitle => "ПУМБ";

        protected override IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages)
        {
            string pageText = string.Join(Environment.NewLine, pages);


            List<Pumb_Row> converted = new List<Pumb_Row>();


            using (var csv = new CsvReader(new StringReader(pageText), DefaultCsvReaderConfig))
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

                var operationCurrency = "UAH";
                var operationAmount = GetDouble(item.OperationAmount.Replace("UAH", string.Empty).Trim());
                var cardCurrenyAmount = GetDouble(item.CardCurrenyAmount.Replace("UAH", string.Empty).Trim());
                var commisionAmount = GetDouble(item.CommisionAmount.Replace("UAH", string.Empty).Trim());

                var tr = new BankTransaction
                {
                    Commission = commisionAmount,
                    OperationCurrency = operationCurrency,
                    OperationAmount = GetDouble($"{sign}{operationAmount}"),
                    CardCurrencyAmount = GetDouble($"{sign}{cardCurrenyAmount}"),

                    Description = $"{item.Details} {item.TransactionType}",
                    Date = item.Date
                };
                transactions.Add(tr);
            }

            return transactions;
        }
    }

    public class Pumb_Row
    {
        [Name("Дата та час операції")]
        public DateTime Date { get; set; }

        [Name("Сума операції")]
        public string OperationAmount { get; set; }


        [Name("Дата виконання (Дата постінгу)")]
        public string ProcessingDate { get; set; }

        [Name("Сума у валюті рахунку")]
        public string CardCurrenyAmount { get; set; }


        [Name("Сума комісій")]
        public string CommisionAmount { get; set; }


        [Name("Номер картки")]
        public string CardNumber { get; set; }

        [Name("Деталі операції")]
        public string Details { get; set; }

        [Name("Опис операції")]
        public string TransactionType { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
