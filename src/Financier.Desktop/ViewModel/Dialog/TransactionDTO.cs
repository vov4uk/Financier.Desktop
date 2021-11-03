using Financier.DataAccess.Data;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransactionDTO : BaseDTO
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
        private ObservableCollection<TransactionDTO> subTransactions = new ObservableCollection<TransactionDTO>();
        private long unSplitAmount;

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

        public bool IsOriginalFromAmountVisible =>
            currency != null && account != null && currency?.Id != account?.Currency.Id;

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
                    return
                        $"1{currency?.Name}={Rate:F5}{account?.Currency.Name}, 1{account?.Currency?.Name}={d:F5}{currency?.Name}";
                }

                return "N/A";
            }
        }

        public long RealFromAmount => Math.Abs(FromAmount) * (IsAmountNegative ? -1 : 1);

        public long SplitAmount
        {
            get { return subTransactions?.Sum(x => x.fromAmount) ?? 0; }
        }

        public ObservableCollection<TransactionDTO> SubTransactions
        {
            get => subTransactions;
            set
            {
                subTransactions = value;
                RaisePropertyChanged(nameof(SubTransactions));
                RecalculateUnSplitAmount();
            }
        }

        public long UnsplitAmount
        {
            get => unSplitAmount;
            set
            {
                unSplitAmount = value;
                RaisePropertyChanged(nameof(UnsplitAmount));
            }
        }

        private void RecalculateRate()
        {
            if (originalFromAmount != null && originalFromAmount != 0)
                Rate = Math.Abs(fromAmount / 100.0 / (originalFromAmount.Value / 100.0));
        }

        public void RecalculateUnSplitAmount()
        {
            if (!IsSubTransaction)
                UnsplitAmount = RealFromAmount - SplitAmount;
            else
                UnsplitAmount = ParentTransactionUnSplitAmount - RealFromAmount;
        }
    }
}