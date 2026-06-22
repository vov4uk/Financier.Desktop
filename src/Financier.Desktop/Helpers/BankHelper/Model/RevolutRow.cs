using CsvHelper.Configuration.Attributes;
using Financier.Desktop.Wizards;
using System;

namespace Financier.Desktop.Helpers.BankHelper.Model
{
    public class RevolutRow
    {
        [Name("Rodzaj", "Type")]
        public string Type { get; set; }

        [Name("Produkt", "Product")]
        public string Product { get; set; }

        [Name("Data rozpoczęcia", "Started Date"), TypeConverter(typeof(DateTimeConvert))]
        public DateTime StartDate { get; set; }

        [Name("Data zrealizowania", "Completed Date")]
        public string CompletionDate { get; set; }

        [Name("Opis", "Description")]
        public string Description { get; set; }

        [Name("Kwota", "Amount")]
        public double Amount { get; set; }

        [Name("Opłata", "Fee")]
        public double Fee { get; set; }

        [Name("Waluta", "Currency")]
        public string Currency { get; set; }

        [Name("State", "State")]
        public string State { get; set; }

        [Name("Saldo", "Balance")]
        public string Balance { get; set; }
    }
}
