using CsvHelper.Configuration.Attributes;
using MiniExcelLibs.Attributes;
using Newtonsoft.Json;
using System;

namespace Financier.Desktop.Helpers.Model
{
    public class Abank_Row
    {
        [Index(0)]
        public string Date { get; set; }

        [Index(2)]
        public string Details { get; set; }

        [Index(3)]
        public string MCC { get; set; }

        [Index(4)]
        public string CardCurrencyAmount { get; set; }

        [Index(5)]
        public string OperationAmount { get; set; }

        [Index(6)]
        public string OperationCurrency { get; set; }

        [Index(7)]
        public string ExchangeRate { get; set; }

        [Index(8)]
        public string Commision { get; set; }

        [Index(9)]
        public string Cashback { get; set; }

        [Index(10)]
        public string Balance { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
