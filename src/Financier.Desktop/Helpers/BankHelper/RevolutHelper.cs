using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Financier.Common;
using Financier.Common.Localization;
using Financier.Desktop.Helpers.BankHelper.Model;
using Financier.Desktop.Wizards;

namespace Financier.Desktop.Helpers.BankHelper
{
    public class RevolutHelper : IBankHelper
    {
        public string BankTitle => LocalizationService.Instance.revolut;

        public IEnumerable<BankTransaction> ParseReport(string filePath)
        {
            if (!File.Exists(filePath))
                return Array.Empty<BankTransaction>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null,
                HasHeaderRecord = true,
            };

            using FileStream file = File.OpenRead(filePath);
            using StreamReader reader = new StreamReader(file, Encoding.UTF8);
            using var csv = new CsvReader(reader, config);

            return csv.GetRecords<RevolutRow>()
                .Select(r => new BankTransaction
                {
                    Date = r.StartDate,
                    Description = r.Description,
                    CardCurrencyAmount = r.Amount,
                    OperationAmount = r.Amount,
                    OperationCurrency = r.Currency,
                    Commission = r.Fee,
                    Balance = DoubleUtils.GetDouble(r.Balance),
                })
                .OrderBy(t => t.Date)
                .ToList();
        }
    }
}
