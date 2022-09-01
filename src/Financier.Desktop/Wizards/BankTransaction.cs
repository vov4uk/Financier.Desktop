using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using System;
using System.Diagnostics;

namespace Financier.Desktop.Wizards
{
    [DebuggerDisplay("{Description} : {CardCurrencyAmount} : {Balance}")]
    public class BankTransaction
    {
        [Name("Date and time", "Дата i час операції"), TypeConverter(typeof(DateTimeConvert))]
        public DateTime Date { get; set; }

        [Name("Description", "Деталі операції")]
        public string Description { get; set; }

        [Name("MCC", "MCC"), Optional]
        public string MCC { get; set; }

        [Name("Card currency amount, (UAH)", "Сума в валюті картки (UAH)"), TypeConverter(typeof(DoubleConverter))]
        public double CardCurrencyAmount { get; set; }

        [Name("Operation amount", "Сума в валюті операції")]
        public double OperationAmount { get; set; }

        [Name("Operation currency", "Валюта")]
        public string OperationCurrency { get; set; }

        [Name("Exchange rate", "Курс"), TypeConverter(typeof(DoubleConvert)), Optional]
        public double? ExchangeRate { get; set; }

        [Name("Commission, (UAH)", "Сума комісій (UAH)"), TypeConverter(typeof(DoubleConvert)), Optional]
        public double? Commission { get; set; }

        [Name("Cashback amount, (UAH)", "Сума кешбеку (UAH)"), TypeConverter(typeof(DoubleConvert)), Optional]
        public double? Cashback { get; set; }

        [Name("Balance", "Залишок після операції"), Optional]
        public double Balance { get; set; } = 0.0;
    }
}