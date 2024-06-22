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
    public class PireusHelper : BankPdfHelperBase
    {
        public override string BankTitle => "Pireus";

        protected override IEnumerable<BankTransaction> ParseTransactionsTable(IEnumerable<string> pages)
        {
            var transactions = new List<BankTransaction>();


            List<Pireus_Row> converted = new List<Pireus_Row>();

            foreach (var page in pages)
            {
                using (var csv = new CsvReader(new StringReader(page), DefaultCsvReaderConfig))
                {
                    var records = csv.GetRecords<Pireus_Row>();

                    var batch = records.Take(1000).ToList();
                    converted.AddRange(batch);
                }
            }


            foreach (var item in converted)
            {
                var operationCurrency = item.OperationCurrency;
                var operationAmount = GetDouble(item.OperationAmount);
                var cardCurrencyAmount = GetDouble(item.CardCurrencyAmount);

                var bt = new BankTransaction
                {
                    Balance = GetDouble(item.Balance),

                    Commission = GetDouble(item.Commision),
                    OperationCurrency = operationAmount != cardCurrencyAmount ? operationCurrency : null,
                    OperationAmount = operationAmount,
                    CardCurrencyAmount = cardCurrencyAmount,
                    Description = item.Details,
                    Date = item.Date
                };

                transactions.Add(bt);
            }

            return transactions.OrderByDescending(x => x.Date);
        }
    }

    public class Pireus_Row
    {
        [Name("Дата та час транзакції")]
        public DateTime Date { get; set; }

        [Name("Сума транзакції")]
        public string OperationAmount { get; set; }

        [Name("Валюта транзакції")]
        public string OperationCurrency { get; set; }

        [Name("Дата виконання операції")]
        public string ProcessingDate { get; set; }

        [Name("Сума операції у валюті рахунку з урахуванням комісії")]
        public string CardCurrencyAmount { get; set; }

        [Name("Комісія")]
        public string Commision { get; set; }

        [Name("Номер карти / номер рахунку")]
        public string CardNumber { get; set; }

        [Name("Деталі операції")]
        public string Details { get; set; }

        [Name("Баланс")]
        public string Balance { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
