using System;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace Financier.Desktop.Helpers.Model
{
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
