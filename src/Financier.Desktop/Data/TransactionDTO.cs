using Financier.DataAccess.Data;
using Financier.Converters;
using Financier.Desktop.Wizards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Financier.Common.Model;
using Financier.Common.Entities;

namespace Financier.Desktop.Data
{
    public class TransactionDto : BaseTransactionDto
    {
        private AccountFilterModel account;
        private int accountId;
        private CategoryModel category;
        private int? categoryId;
        private CurrencyModel currency;
        private int? originalCurrencyId;
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
            id = 0;
            fromAmount = x.FromAmount;
            isAmountNegative = x.FromAmount < 0;
            originalFromAmount = x.OriginalFromAmount ?? 0;
            originalCurrencyId = x.OriginalCurrencyId;
            note = x.Note;
            locationId = x.LocationId;
            projectId = x.ProjectId;
            categoryId = x.CategoryId;
            category = default;
        }

        public TransactionDto(Transaction transaction, IEnumerable<Transaction> subTransactions)
            : this(transaction)
        {
            SubTransactions = new ObservableCollection<TransactionDto>(subTransactions.Select(x => new TransactionDto(x)));
        }

        public TransactionDto(Transaction transaction)
        {
            id = transaction.Id;
            accountId = transaction.FromAccountId;
            categoryId = transaction.CategoryId;
            payeeId = transaction.PayeeId;
            originalCurrencyId = transaction.OriginalCurrencyId;
            originalFromAmount = transaction.OriginalFromAmount;
            locationId = transaction.LocationId;
            projectId = transaction.ProjectId;
            note = transaction.Note;
            fromAmount = transaction.FromAmount;
            isAmountNegative = transaction.FromAmount <= 0;
            date = UnixTimeConverter.Convert(transaction.DateTime).Date;
            time = UnixTimeConverter.Convert(transaction.DateTime);
        }

        public AccountFilterModel Account
        {
            get => account ??= DbManual.Account?.FirstOrDefault(x => x.Id == AccountId);
            set
            {
                account = value;
                RaisePropertyChanged(nameof(Account));
                RaisePropertyChanged(nameof(IsOriginalFromAmountVisible));
                RaisePropertyChanged(nameof(RateString));
                RaisePropertyChanged(nameof(AccountCurrency));
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

        public CurrencyModel AccountCurrency
        {
            get => DbManual.Currencies?.FirstOrDefault(x => x.Id == (Account != null ? Account.CurrencyId : 0));
        }

        public CategoryModel Category
        {
            get => category ??= DbManual.Category?.FirstOrDefault(x => x.Id == CategoryId);
            set
            {
                category = value;
                RaisePropertyChanged(nameof(Category));

                if (category != null && category.Id > 0)
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

        public CurrencyModel OriginalCurrency
        {
            get => currency ??= DbManual.Currencies?.FirstOrDefault(x => x.Id == OriginalCurrencyId);
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
            get => originalCurrencyId;
            set
            {
                originalCurrencyId = value;
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
                    var localCurrency = DbManual.Currencies?.FirstOrDefault(x => x.Id == account?.Id);
                    return $"1{currency?.Name}={Rate:F5}{localCurrency?.Name}, 1{localCurrency?.Name}={d:F5}{currency?.Name}";
                }

                return "N/A";
            }
        }

        public long RealFromAmount => Math.Abs(IsOriginalFromAmountVisible ? (OriginalFromAmount ?? 0 ): FromAmount) * (IsAmountNegative ? -1 : 1);

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
            {
                UnsplitAmount = RealFromAmount - SplitAmount;
            }
            else
            {
                UnsplitAmount = ParentTransactionUnSplitAmount - RealFromAmount;
            }
        }

        private void RecalculateRate()
        {
            if (originalFromAmount != null && originalFromAmount != 0)
                Rate = Math.Abs(fromAmount / 100.0 / (originalFromAmount.Value / 100.0));
        }
    }
}