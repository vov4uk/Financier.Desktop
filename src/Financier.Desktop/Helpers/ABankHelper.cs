using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Financier.Desktop.Wizards;
using Newtonsoft.Json;
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
                var operationAmount = GetDouble(item.OperationAmount.Replace(Space, string.Empty));
                var cardCurrencyAmount = GetDouble(item.CardCurrencyAmount.Replace(Space, string.Empty));

                var bt = new BankTransaction
                {
                    Balance = GetDouble(item.Balance.Replace(Space, string.Empty)),
                    Cashback = GetDouble(item.Cashback.Replace(Space, string.Empty)),
                    Commission = GetDouble(item.Commision.Replace(Space, string.Empty)),
                    ExchangeRate = GetDouble(item.ExchangeRate.Replace(Space, string.Empty)),
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

    }

    public class Abank_Row
    {
        [Name("Дата і час операції")]
        public DateTime Date { get; set; }

        [Name("Сума у валюті операції")]
        public string OperationAmount { get; set; }

        [Name("Валюта")]
        public string OperationCurrency { get; set; }

        [Name("Курс")]
        public string ExchangeRate { get; set; }

        [Name("Сума комісій (UAH)")]
        public string Commision { get; set; }

        [Name("Сума кешбеку (UAH)")]
        public string Cashback { get; set; }

        [Name("Сума у валюті карти (UAH)")]
        public string CardCurrencyAmount { get; set; }

        [Name("МСС")]
        public string MCC { get; set; }

        [Name("Деталі операції")]
        public string Details { get; set; }

        [Name("Залишок після операціЇ")]
        public string Balance { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
