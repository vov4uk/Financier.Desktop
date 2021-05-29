using System;
using System.Diagnostics;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace Financier.DataAccess.Monobank
{
    [DebuggerDisplay("{Description} : {CardCurrencyAmount} : {Balance}")]
    public class MonoTransaction
    {

        [Name("Date and time", "Дата i час операції"), TypeConverter(typeof(DateTimeConvert)), Index(0)]
        public DateTime Date { get; set; }

        [Name("Description", "Деталі операції"), Index(1)]
        public string Description { get; set; }

        [Name("MCC", "MCC"), Index(2)]
        public string MCC { get; set; }

        [Name("Card currency amount, (UAH)", "Сума в валюті картки (UAH)"), TypeConverter(typeof(DoubleConverter)), Index(3)]
        public double CardCurrencyAmount { get; set; }

        [Name("Operation amount", "Сума в валюті операції"), Index(4)]
        public double OperationAmount { get; set; }

        [Name("Operation currency", "Валюта"), Index(5)]
        public string OperationCurrency { get; set; }

        [Name("Exchange rate", "Курс"), TypeConverter(typeof(DoubleConvert)), Index(6)]
        public double? ExchangeRate { get; set; }

        [Name("Commission, (UAH)", "Сума комісій (UAH)"), TypeConverter(typeof(DoubleConvert)), Index(7)]
        public double? Commission { get; set; }

        [Name("Cashback amount, (UAH)", "Сума кешбеку (UAH)"), TypeConverter(typeof(DoubleConvert)), Index(8)]
        public double? Cashback { get; set; }

        [Name("Balance", "Залишок після операції"), Index(9)]
        public double Balance { get; set; }
    }
}
