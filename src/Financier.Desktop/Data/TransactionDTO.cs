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
        private AccountFilterModel fromAccount;
        private int fromAccountId;
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
        private ObservableCollection<BaseTransactionDto> subTransactions = new ObservableCollection<BaseTransactionDto>();
        private long unSplitAmount;

        public TransactionDto() { }

        public TransactionDto(FinancierTransactionDto x)
        {
            id = 0;
            fromAmount = x.FromAmount;
            isAmountNegative = x.FromAmount < 0;
            OriginalFromAmount = x.OriginalFromAmount ?? 0;
            OriginalCurrencyId = x.OriginalCurrencyId;
            note = x.Note;
            locationId = x.LocationId;
            projectId = x.ProjectId;
            categoryId = x.CategoryId;
            category = default;
        }

        public TransactionDto(Transaction transaction, IEnumerable<Transaction> subTransactions)
            : this(transaction)
        {
            var list = new List<BaseTransactionDto>();
            foreach (var t in subTransactions)
            {
                if (t.ToAccountId > 0 && t.CategoryId == 0 && t.FromAccountId > 0)
                {
                    list.Add(new TransferDto(t));
                }
                else
                {
                    list.Add(new TransactionDto(t));
                }
            }


            SubTransactions = new ObservableCollection<BaseTransactionDto>(list);
        }

        public TransactionDto(Transaction transaction)
        {
            id = transaction.Id;
            fromAccountId = transaction.FromAccountId;
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

        public AccountFilterModel FromAccount
        {
            get => fromAccount ??= DbManual.Account?.FirstOrDefault(x => x.Id == FromAccountId);
            set
            {
                if (SetProperty(ref fromAccount, value))
                {
                    RaisePropertyChanged(nameof(FromAccount));
                    RaisePropertyChanged(nameof(IsOriginalFromAmountVisible));
                    RaisePropertyChanged(nameof(RateString));
                    RaisePropertyChanged(nameof(FromAccountCurrency));
                }
            }
        }

        public int FromAccountId
        {
            get => fromAccountId;
            set
            {
                if (SetProperty(ref fromAccountId, value))
                {
                    RaisePropertyChanged(nameof(FromAccountId));
                }
            }
        }

        public CurrencyModel FromAccountCurrency
        {
            get => DbManual.Currencies?.FirstOrDefault(x => x.Id == (FromAccount != null ? FromAccount.CurrencyId : 0));
        }

        public CategoryModel Category
        {
            get => category ??= DbManual.Category?.FirstOrDefault(x => x.Id == CategoryId);
            set
            {
                if (SetProperty(ref category, value))
                {
                    RaisePropertyChanged(nameof(Category));
                }
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
                if (SetProperty(ref categoryId, value))
                {
                    RaisePropertyChanged(nameof(CategoryId));
                    RaisePropertyChanged(nameof(IsSplitCategory));
                }
            }
        }

        public long FromAmount
        {
            get => fromAmount;
            set
            {
                if (SetProperty(ref fromAmount, value))
                {
                    RaisePropertyChanged(nameof(FromAmount));
                    RecalculateRate();
                    RecalculateUnSplitAmount();
                }
            }
        }

        public override bool IsAmountNegative
        {
            get => isAmountNegative;
            set
            {
                if (SetProperty(ref isAmountNegative, value))
                {
                    RaisePropertyChanged(nameof(IsAmountNegative));
                    RecalculateUnSplitAmount();
                }
            }
        }

        public bool IsOriginalFromAmountVisible => OriginalCurrency != null && OriginalCurrency.Id != null && FromAccount != null && OriginalCurrency.Id != FromAccount.CurrencyId;

        public bool IsSplitCategory => categoryId == -1;

        public int? LocationId
        {
            get => locationId;
            set
            {
                if (SetProperty(ref locationId, value))
                {
                    RaisePropertyChanged(nameof(LocationId));
                }
            }
        }

        public CurrencyModel OriginalCurrency
        {
            get => currency ??= DbManual.Currencies?.FirstOrDefault(x => x.Id == OriginalCurrencyId);
            set
            {
                if (SetProperty(ref currency, value))
                {
                    RaisePropertyChanged(nameof(OriginalCurrency));
                    RaisePropertyChanged(nameof(IsOriginalFromAmountVisible));
                    RaisePropertyChanged(nameof(RateString));
                }
            }
        }

        public int? OriginalCurrencyId
        {
            get => originalCurrencyId;
            set
            {
                if (SetProperty(ref originalCurrencyId, value))
                {
                    RaisePropertyChanged(nameof(OriginalCurrencyId));
                }
            }
        }

        public long? OriginalFromAmount
        {
            get => originalFromAmount;
            set
            {
                if (SetProperty(ref originalFromAmount, value))
                {
                    RaisePropertyChanged(nameof(OriginalFromAmount));
                    RecalculateRate();
                }
            }
        }

        public long ParentTransactionUnSplitAmount
        {
            get => parentTransactionSplitAmount;
            set
            {
                if (SetProperty(ref parentTransactionSplitAmount, value))
                {
                    RaisePropertyChanged(nameof(ParentTransactionUnSplitAmount));
                    RecalculateUnSplitAmount();
                }
            }
        }

        public int? PayeeId
        {
            get => payeeId;
            set
            {
                if (SetProperty(ref payeeId, value))
                {
                    RaisePropertyChanged(nameof(PayeeId));
                }
            }
        }

        public int? ProjectId
        {
            get => projectId;
            set
            {
                if (SetProperty(ref projectId, value))
                {
                    RaisePropertyChanged(nameof(ProjectId));
                }
            }
        }

        public string RateString
        {
            get
            {
                if (Rate != 0)
                {
                    var d = 1.0 / Rate;
                    var localCurrency = DbManual.Currencies?.FirstOrDefault(x => x.Id == fromAccount?.Id);
                    return $"1{currency?.Name}={Rate:F5}{localCurrency?.Name}, 1{localCurrency?.Name}={d:F5}{currency?.Name}";
                }

                return "N/A";
            }
        }

        public override long RealFromAmount => Math.Abs(IsOriginalFromAmountVisible ? (OriginalFromAmount ?? 0 ): FromAmount) * (IsAmountNegative ? -1 : 1);

        public override string SubTransactionTitle => Category?.Title ?? string.Empty;

        public long SplitAmount => subTransactions?.Sum(x => x.RealFromAmount) ?? 0;

        public ObservableCollection<BaseTransactionDto> SubTransactions
        {
            get => subTransactions;
            private set
            {
                if (SetProperty(ref subTransactions, value))
                {
                    RaisePropertyChanged(nameof(SubTransactions));
                    RecalculateUnSplitAmount();
                }
            }
        }

        public long UnsplitAmount
        {
            get => unSplitAmount;
            private set
            {
                if (SetProperty(ref unSplitAmount, value))
                {
                    RaisePropertyChanged(nameof(UnsplitAmount));
                }
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

        internal void RecalculateRate()
        {
            if (originalFromAmount != null && originalFromAmount != 0)
                Rate = Math.Abs(fromAmount / 100.0 / (originalFromAmount.Value / 100.0));
        }
    }
}