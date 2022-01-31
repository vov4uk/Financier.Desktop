using Financier.DataAccess.Data;
using Financier.Desktop.Converters;
using Financier.Desktop.Wizards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.Data
{
    public class TransactionDto : BaseTransactionDto
    {
        private Account account;
        private int accountId;
        private Category category;
        private int? categoryId;
        private Currency currency;
        private int? currencyId;
        private long fromAmount;
        private bool isAmountNegative;
        private int? locationId;
        private long? originalFromAmount;
        private long parentTransactionSplitAmount;
        private int? payeeId;
        private int? projectId;
        private ObservableCollection<TransactionDto> subTransactions = new ObservableCollection<TransactionDto>();
        private long unSplitAmount;

        public TransactionDto() { }

        public TransactionDto(FinancierTransactionDto x)
        {
            Id = 0;
            FromAmount = x.FromAmount;
            IsAmountNegative = x.FromAmount < 0;
            OriginalFromAmount = x.OriginalFromAmount ?? 0;
            OriginalCurrencyId = x.OriginalCurrencyId;
            Note = x.Note;
            LocationId = x.LocationId;
            ProjectId = x.ProjectId;
            CategoryId = x.CategoryId;
            Category = default;
        }

        public TransactionDto(Transaction transaction, IEnumerable<Transaction> subTransactions)
            : this(transaction)
        {
            SubTransactions = new ObservableCollection<TransactionDto>(subTransactions.Select(x => new TransactionDto(x)));
        }

        public TransactionDto(Transaction transaction)
        {
            Id = transaction.Id;
            Account = transaction.FromAccount;
            AccountId = transaction.FromAccountId;
            CategoryId = transaction.CategoryId;
            Category = transaction.Category;
            PayeeId = transaction.PayeeId;
            OriginalCurrencyId = transaction.OriginalCurrencyId;
            OriginalCurrency = transaction.OriginalCurrency;
            OriginalFromAmount = transaction.OriginalFromAmount;
            LocationId = transaction.LocationId;
            ProjectId = transaction.ProjectId;
            Note = transaction.Note;
            FromAmount = transaction.FromAmount;
            IsAmountNegative = transaction.FromAmount <= 0;
            Date = UnixTimeConverter.Convert(transaction.DateTime).Date;
            Time = UnixTimeConverter.Convert(transaction.DateTime);
        }

        public Account Account
        {
            get => account;
            set
            {
                account = value;
                RaisePropertyChanged(nameof(Account));
                RaisePropertyChanged(nameof(IsOriginalFromAmountVisible));
                RaisePropertyChanged(nameof(RateString));
            }
        }

        public int AccountId
        {
            get => accountId;
            set
            {
                accountId = value;
                RaisePropertyChanged(nameof(AccountId));
            }
        }

        public Category Category
        {
            get => category;
            set
            {
                category = value;
                RaisePropertyChanged(nameof(Category));

                if (category != null)
                {
                    IsAmountNegative = category.Type == 0;
                }
            }
        }

        public int? CategoryId
        {
            get => categoryId;
            set
            {
                categoryId = value;
                RaisePropertyChanged(nameof(CategoryId));
                RaisePropertyChanged(nameof(IsSplitCategory));
            }
        }

        public long FromAmount
        {
            get => fromAmount;
            set
            {
                fromAmount = value;
                RaisePropertyChanged(nameof(FromAmount));
                RecalculateRate();
                RecalculateUnSplitAmount();
            }
        }

        public bool IsAmountNegative
        {
            get => isAmountNegative;
            set
            {
                isAmountNegative = value;
                RaisePropertyChanged(nameof(IsAmountNegative));
                RecalculateUnSplitAmount();
            }
        }

        public bool IsOriginalFromAmountVisible => OriginalCurrency != null && Account != null && OriginalCurrency.Id != Account.CurrencyId;

        public bool IsSplitCategory => categoryId == -1;

        public bool IsSubTransaction { get; set; }

        public int? LocationId
        {
            get => locationId;
            set
            {
                locationId = value;
                RaisePropertyChanged(nameof(LocationId));
            }
        }

        public Currency OriginalCurrency
        {
            get => currency;
            set
            {
                currency = value;
                RaisePropertyChanged(nameof(OriginalCurrency));
                RaisePropertyChanged(nameof(IsOriginalFromAmountVisible));
                RaisePropertyChanged(nameof(RateString));
            }
        }

        public int? OriginalCurrencyId
        {
            get => currencyId;
            set
            {
                currencyId = value;
                RaisePropertyChanged(nameof(OriginalCurrencyId));
            }
        }

        public long? OriginalFromAmount
        {
            get => originalFromAmount;
            set
            {
                originalFromAmount = value;
                RaisePropertyChanged(nameof(OriginalFromAmount));
                RecalculateRate();
            }
        }

        public long ParentTransactionUnSplitAmount
        {
            get => parentTransactionSplitAmount;
            set
            {
                parentTransactionSplitAmount = value;
                RaisePropertyChanged(nameof(ParentTransactionUnSplitAmount));
                RecalculateUnSplitAmount();
            }
        }

        public int? PayeeId
        {
            get => payeeId;
            set
            {
                payeeId = value;
                RaisePropertyChanged(nameof(PayeeId));
            }
        }

        public int? ProjectId
        {
            get => projectId;
            set
            {
                projectId = value;
                RaisePropertyChanged(nameof(ProjectId));
            }
        }

        public string RateString
        {
            get
            {
                if (Rate != 0)
                {
                    var d = 1.0 / Rate;
                    return $"1{currency?.Name}={Rate:F5}{account?.Currency.Name}, 1{account?.Currency?.Name}={d:F5}{currency?.Name}";
                }

                return "N/A";
            }
        }

        public long RealFromAmount => Math.Abs(IsOriginalFromAmountVisible ? OriginalFromAmount.Value : FromAmount) * (IsAmountNegative ? -1 : 1);

        public long SplitAmount
        {
            get { return subTransactions?.Sum(x => x.RealFromAmount) ?? 0; }
        }

        public ObservableCollection<TransactionDto> SubTransactions
        {
            get => subTransactions;
            private set
            {
                subTransactions = value;
                RaisePropertyChanged(nameof(SubTransactions));
                RecalculateUnSplitAmount();
            }
        }

        public long UnsplitAmount
        {
            get => unSplitAmount;
            private set
            {
                unSplitAmount = value;
                RaisePropertyChanged(nameof(UnsplitAmount));
            }
        }

        public void RecalculateUnSplitAmount()
        {
            if (!IsSubTransaction)
                UnsplitAmount = RealFromAmount - SplitAmount;
            else
                UnsplitAmount = ParentTransactionUnSplitAmount - RealFromAmount;
        }

        private void RecalculateRate()
        {
            if (originalFromAmount != null && originalFromAmount != 0)
                Rate = Math.Abs(fromAmount / 100.0 / (originalFromAmount.Value / 100.0));
        }
    }
}