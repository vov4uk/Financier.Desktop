using Financier.DataAccess.Data;
using System;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransferDto : BaseTransactionDto
    {
        private Account fromAccount;
        private int fromAccountId;
        private long fromAmount;
            private Account toAccount;
        private int toAccountId;
        private long toAmount;

        public TransferDto(Transaction transaction)
        public Account FromAccount
        {
            get => fromAccount;
            set
            {
                fromAccount = value;
                RaisePropertyChanged(nameof(FromAccount));
                RaisePropertyChanged(nameof(RateString));
                RaisePropertyChanged(nameof(IsToAmountVisible));
            }
        }

        public int FromAccountId
        {
            get => fromAccountId;
            set
            {
                fromAccountId = value;
                RaisePropertyChanged(nameof(FromAccountId));
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
            }
        }

        public bool IsToAmountVisible => fromAccount != null && toAccount != null && fromAccount.CurrencyId != toAccount.CurrencyId;

        public string RateString
        {
            get
            {
                if (Rate != 0)
                {
                    var d = 1.0 / Rate;
                    return $"1{toAccount?.Currency.Name}={Rate:F5}{fromAccount?.Currency.Name}, 1{fromAccount?.Currency?.Name}={d:F5}{toAccount?.Currency.Name}";
                }
                return "N/A";
            }
        }

        public Account ToAccount
        {
            get => toAccount;
            set
            {
                toAccount = value;
                RaisePropertyChanged(nameof(ToAccount));
                RaisePropertyChanged(nameof(RateString));
                RaisePropertyChanged(nameof(IsToAmountVisible));
            }
        }

        public int ToAccountId
        {
            get => toAccountId;
            set
            {
                toAccountId = value;
                RaisePropertyChanged(nameof(ToAccountId));
            }
        }

        public long ToAmount
        {
            get => toAmount;
            set
            {
                toAmount = value;
                RaisePropertyChanged(nameof(ToAmount));
                RecalculateRate();
            }
        }

        private void RecalculateRate()
        {
            if (fromAmount != 0 && toAmount != 0)
            {
                Rate = Math.Abs(fromAmount / 100.0 / (toAmount / 100.0));
            }
        }
    }
}
