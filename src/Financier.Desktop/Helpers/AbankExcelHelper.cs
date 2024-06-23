
using Financier.Desktop.Wizards;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Desktop.Helpers
{
    public class AbankExcelHelper : IBankHelper
    {
        protected const string Space = " ";
        public string BankTitle => "A-Bank";

        public IEnumerable<BankTransaction> ParseReport(string filePath)
        {
            var rows = MiniExcel.Query(filePath, useHeaderRow: true, excelType: ExcelType.XLSX, startCell: "A20").Cast<IDictionary<string, object>>().ToList();

            var transactions = new List<BankTransaction>();

            foreach (var item in rows.Where(x => x["Сума в валюті операції"] != null))
            {

                var operationCurrency = Convert.ToString(item["Валюта"]);
                var operationAmount = ToDouble(item["Сума в валюті операції"]);
                var cardCurrencyAmount = ToDouble(item["Сума в валюті картки (UAH)"]);

                var bt = new BankTransaction
                {
                    Balance = ToDouble(item["Залишок після операції"]),
                    Cashback = ToDouble(item["Сума кешбеку (UAH)"]),
                    Commission = ToDouble(item["Сума комісій (UAH)"]),
                    ExchangeRate = ToDouble(item["Курс"]),
                    OperationCurrency = operationAmount != cardCurrencyAmount ? operationCurrency : null,
                    OperationAmount = operationAmount,
                    CardCurrencyAmount = cardCurrencyAmount,
                    MCC = Convert.ToString(item["MCC"]),
                    Description = Convert.ToString(item["Деталі операції"]),
                    Date = Convert.ToDateTime(item["Дата i час операції"])
                };
                transactions.Add(bt);
            }

            return transactions;
        }


        private double ToDouble(object val)
        {
            return BankPdfHelperBase.GetDouble(Convert.ToString(val).Replace(Space, string.Empty));
        }
    }
}
