using Financier.Common.Utils;

namespace Financier.Common.Model
{
    public class AccountModel : BaseModel
    {
        public int Id { get; set; } = -1;

        public bool IsActive { get; set; } = true;

        public string Title { get; set; }

        public long LastTransactionDate { get; set; }

        public int CurrencyId { get; set; }

        public string Type { get; set; }

        public long TotalAmount { get; set; }

        public int SortOrder { get; set; }

        public bool IsIncludeIntoTotals { get; set; } = true;

        public CurrencyModel Currency { get; set; }

        public string AmountTitle => BlotterUtils.SetAmountText(Currency, TotalAmount, false);
    }
}
