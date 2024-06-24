using System;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace Financier.Desktop.Helpers.Model
{
    public class Pumb_Row
    {
        [Index(0)]
        public DateTime Date { get; set; }

        [Index(1)]
        public string OperationAmount { get; set; }


        [Index(2)]
        public string ProcessingDate { get; set; }

        [Index(3)]
        public string CardCurrenyAmount { get; set; }


        [Index(4)]
        public string CommisionAmount { get; set; }


        [Index(5)]
        public string CardNumber { get; set; }

        [Index(6)]
        public string Details { get; set; }

        [Index(7)]
        public string TransactionType { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
