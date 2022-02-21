using Financier.Common.Utils;
using Financier.DataAccess.Data;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class AccountModel : AccountFilterModel
    {
        public long LastTransactionDate { get; set; }

        public bool IsIncludeIntoTotals { get; set; } = true;

        public CurrencyModel Currency { get; set; }

        public string AmountTitle => BlotterUtils.SetAmountText(Currency, TotalAmount, false);

        public AccountModel() { }

        public AccountModel(Account acc)
        {
            Id = acc.Id;
            Title = acc.Title;
            CurrencyId = acc.CurrencyId;
            IsActive = acc.IsActive;
            IsIncludeIntoTotals = acc.IsIncludeIntoTotals;
            LastTransactionDate = acc.LastTransactionDate;
            SortOrder = acc.SortOrder;
            TotalAmount = acc.TotalAmount;
            Type = acc.Type;
            Currency = acc.Currency != null ? new CurrencyModel(acc.Currency) : default;
        }
    }
}
