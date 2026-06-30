using Financier.DataAccess.Data;
using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class AccountDto : BindableBase
    {
        private string title;
        private bool isActive;
        private string type;
        private int currencyId;
        private string cardIssuer;
        private string issuer;
        private string number;
        private long limitAmount;
        private int sortOrder;
        private bool isIncludeIntoTotals;
        private string note;
        private int closingDay;
        private int paymentDay;
        private long openingAmount;

        public AccountDto() { }

        public AccountDto(Account account)
        {
            Id = account.Id;
            Title = account.Title;
            IsActive = account.IsActive;
            Type = account.Type;
            CurrencyId = account.CurrencyId;
            CardIssuer = account.CardIssuer;
            Issuer = account.Issuer;
            Number = account.Number;
            LimitAmount = account.LimitAmount;
            SortOrder = account.SortOrder;
            IsIncludeIntoTotals = account.IsIncludeIntoTotals;
            Note = account.Note;
            ClosingDay = account.ClosingDay;
            PaymentDay = account.PaymentDay;
        }

        public int Id { get; set; }

        public string Title
        {
            get => title;
            set { SetProperty(ref title, value, nameof(Title)); }
        }

        public bool IsActive
        {
            get => isActive;
            set { SetProperty(ref isActive, value, nameof(IsActive)); }
        }

        public string Type
        {
            get => type;
            set { SetProperty(ref type, value, nameof(Type)); }
        }

        public int CurrencyId
        {
            get => currencyId;
            set { SetProperty(ref currencyId, value, nameof(CurrencyId)); }
        }

        public string CardIssuer
        {
            get => cardIssuer;
            set { SetProperty(ref cardIssuer, value, nameof(CardIssuer)); }
        }

        public string Issuer
        {
            get => issuer;
            set { SetProperty(ref issuer, value, nameof(Issuer)); }
        }

        public string Number
        {
            get => number;
            set { SetProperty(ref number, value, nameof(Number)); }
        }

        public long LimitAmount
        {
            get => limitAmount;
            set { SetProperty(ref limitAmount, value, nameof(LimitAmount)); }
        }

        public int SortOrder
        {
            get => sortOrder;
            set { SetProperty(ref sortOrder, value, nameof(SortOrder)); }
        }

        public bool IsIncludeIntoTotals
        {
            get => isIncludeIntoTotals;
            set { SetProperty(ref isIncludeIntoTotals, value, nameof(IsIncludeIntoTotals)); }
        }

        public string Note
        {
            get => note;
            set { SetProperty(ref note, value, nameof(Note)); }
        }

        public int ClosingDay
        {
            get => closingDay;
            set { SetProperty(ref closingDay, value, nameof(ClosingDay)); }
        }

        public int PaymentDay
        {
            get => paymentDay;
            set { SetProperty(ref paymentDay, value, nameof(PaymentDay)); }
        }

        public long OpeningAmount
        {
            get => openingAmount;
            set { SetProperty(ref openingAmount, value, nameof(OpeningAmount)); }
        }
    }
}
