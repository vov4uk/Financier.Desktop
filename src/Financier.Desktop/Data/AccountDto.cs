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
            set { title = value; RaisePropertyChanged(nameof(Title)); }
        }

        public bool IsActive
        {
            get => isActive;
            set { isActive = value; RaisePropertyChanged(nameof(IsActive)); }
        }

        public string Type
        {
            get => type;
            set { type = value; RaisePropertyChanged(nameof(Type)); }
        }

        public int CurrencyId
        {
            get => currencyId;
            set { currencyId = value; RaisePropertyChanged(nameof(CurrencyId)); }
        }

        public string CardIssuer
        {
            get => cardIssuer;
            set { cardIssuer = value; RaisePropertyChanged(nameof(CardIssuer)); }
        }

        public string Issuer
        {
            get => issuer;
            set { issuer = value; RaisePropertyChanged(nameof(Issuer)); }
        }

        public string Number
        {
            get => number;
            set { number = value; RaisePropertyChanged(nameof(Number)); }
        }

        public long LimitAmount
        {
            get => limitAmount;
            set { limitAmount = value; RaisePropertyChanged(nameof(LimitAmount)); }
        }

        public int SortOrder
        {
            get => sortOrder;
            set { sortOrder = value; RaisePropertyChanged(nameof(SortOrder)); }
        }

        public bool IsIncludeIntoTotals
        {
            get => isIncludeIntoTotals;
            set { isIncludeIntoTotals = value; RaisePropertyChanged(nameof(IsIncludeIntoTotals)); }
        }

        public string Note
        {
            get => note;
            set { note = value; RaisePropertyChanged(nameof(Note)); }
        }

        public int ClosingDay
        {
            get => closingDay;
            set { closingDay = value; RaisePropertyChanged(nameof(ClosingDay)); }
        }

        public int PaymentDay
        {
            get => paymentDay;
            set { paymentDay = value; RaisePropertyChanged(nameof(PaymentDay)); }
        }

        public long OpeningAmount
        {
            get => openingAmount;
            set { openingAmount = value; RaisePropertyChanged(nameof(OpeningAmount)); }
        }
    }
}
