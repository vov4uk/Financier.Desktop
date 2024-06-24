using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;

namespace Financier.Desktop.Helpers.Model
{
    public class Pireus_Row
    {
        [Index(0)]
        public DateTime Date { get; set; }

        [Index(1)]
        public string OperationAmount { get; set; }

        [Index(2)]
        public string OperationCurrency { get; set; }

        [Index(3)]
        public string ProcessingDate { get; set; }

        [Index(4)]
        public string CardCurrencyAmount { get; set; }

        [Index(5)]
        public string Commision { get; set; }

        [Index(6)]
        public string CardNumber { get; set; }

        [Index(7)]
        public string Details { get; set; }

        [Index(8)]
        public string Balance { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
