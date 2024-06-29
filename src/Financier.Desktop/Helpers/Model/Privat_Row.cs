using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using System;

namespace Financier.Desktop.Helpers.Model
{
    public class Privat_Row
    {
        [Index(0)]
        public string Date { get; set; }

        [Index(1)]
        public string Category { get; set; }
        
        [Index(2)]
        public string CardNumber { get; set; }

        [Index(3)]
        public string Details { get; set; }

        [Index(4)]
        public string CardCurrencyAmount { get; set; }

        [Index(5)]
        public string CardCurrency { get; set; }

        [Index(6)]
        public string OperationAmount { get; set; }

        [Index(7)]
        public string OperationCurrency { get; set; }

        [Index(8)]
        public string Balance { get; set; }

        [Index(9)]
        public string BalanceCurrancy { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
