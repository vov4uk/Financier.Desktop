using CsvHelper.Configuration.Attributes;
using MiniExcelLibs.Attributes;
using Newtonsoft.Json;
using System;

namespace Financier.Desktop.Helpers.Model
{
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
