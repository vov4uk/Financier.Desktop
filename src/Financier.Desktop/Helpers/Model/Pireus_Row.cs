using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;

namespace Financier.Desktop.Helpers.Model
{
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
