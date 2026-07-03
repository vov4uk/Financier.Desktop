using System;
using System.Diagnostics.CodeAnalysis;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace Financier.Desktop.Helpers.BankHelper.Model
{
    [ExcludeFromCodeCoverage]
    public class PrivatRow
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
