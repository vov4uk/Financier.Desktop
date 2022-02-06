using Financier.Common.Utils;
using Financier.DataAccess.Utils;

namespace Financier.Common.Model
{
    public class BlotterModel : BaseModel
    {
        public int Id { get; set; }

        public int FromAccountId { get; set; }
        public string FromAccountTitle { get; set; }

        public int? ToAccountId { get; set; }
        public string ToAccountTitle { get; set; }

        public int FromAccountCurrencyId { get; set; }

        public int? ToAccountCurrencyId { get; set; }

        public int? CategoryId { get; set; }
        public string CategoryTitle { get; set; }

        public int? LocationId { get; set; }
        public string Location { get; set; }

        public string Payee { get; set; }

        public string Note { get; set; }

        public int FromAmount { get; set; }
        public int ToAmount { get; set; }

        public long Datetime { get; set; }

        public int? OriginalCurrencyId { get; set; }

        public long OriginalFromAmount { get; set; }

        public int? FromAccountBalance { get; set; }
        public int? ToAccountBalance { get; set; }

        public CurrencyModel FromAccountCurrency { get; set; }
        public CurrencyModel ToAccountCurrency { get; set; }
        public CurrencyModel OriginalCurrency { get; set; }

        public string Type
        {
            get
            {
                if (ToAccountId > 0)
                {
                    return "Transfer";
                }
                else if (CategoryId == -1)
                {
                    return "Share";
                }
                else if (FromAmount > 0)
                {
                    return "Income";
                }
                return "Expense";
            }
        }

        public string AccountTitle
        {
            get
            {
                if (ToAccountId > 0)
                {
                    return $"{FromAccountTitle}{BlotterUtils.TRANSFER_DELIMITER}{ToAccountTitle}";
                }
                return FromAccountTitle;
            }
        }

        public string TransactionTitle => TransactionTitleUtils.GenerateTransactionTitle(Payee, Note, LocationId > 0 ? Location : string.Empty, CategoryId, CategoryTitle, ToAccountId);

        public string AmountTitle
        {
            get
            {
                if (ToAccountId > 0)
                {
                    return BlotterUtils.GetTransferAmountText(FromAccountCurrency, FromAmount, ToAccountCurrency, ToAmount);
                }

                if (OriginalCurrencyId > 0)
                {
                    return BlotterUtils.SetAmountText(OriginalCurrency, OriginalFromAmount, FromAccountCurrency, FromAmount, true);
                }
                return BlotterUtils.SetAmountText(FromAccountCurrency, FromAmount, true);
            }
        }

        public string BalanceTitle
        {
            get
            {
                if (ToAccountId > 0)
                {
                    return BlotterUtils.SetTransferBalanceText(FromAccountCurrency, FromAccountBalance, ToAccountCurrency, ToAccountBalance);
                }
                return BlotterUtils.SetAmountText(FromAccountCurrency, FromAccountBalance ?? 0, false);
            }
        }

        public bool HasNoCategory => Type != "Transfer" && CategoryId == 0;
    }
}