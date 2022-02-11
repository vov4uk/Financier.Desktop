using Financier.DataAccess.Data;
using Financier.Converters;
using System;
using Financier.Common.Model;
using Financier.Common.Entities;
using System.Linq;

namespace Financier.Desktop.Data
{
    public class TransferDto : BaseTransactionDto
    {
        private AccountFilterModel fromAccount;
        private int fromAccountId;
        private long fromAmount;
        private AccountFilterModel toAccount;
        private int toAccountId;
        private long toAmount;

        public TransferDto(Transaction transaction)
        {
            Id = transaction.Id;
            FromAccountId = transaction.FromAccountId;
            ToAccountId = transaction.ToAccountId;
            Note = transaction.Note;
            FromAmount = transaction.FromAmount;
            ToAmount = transaction.ToAmount;
            Date = UnixTimeConverter.Convert(transaction.DateTime).Date;
            Time = UnixTimeConverter.Convert(transaction.DateTime);
        }

        public AccountFilterModel FromAccount
        {
            get => fromAccount;
            set
            {
                fromAccount = value;
                RaisePropertyChanged(nameof(FromAccount));
                RaisePropertyChanged(nameof(RateString));
                RaisePropertyChanged(nameof(IsToAmountVisible));
                RaisePropertyChanged(nameof(FromAccountCurrency));
            }
        }

        public CurrencyModel FromAccountCurrency
        {
            get => DbManual.Currencies.FirstOrDefault(x => x.Id == (FromAccount != null ? FromAccount.CurrencyId : 0));
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
                    return $"1{ToAccountCurrency?.Name}={Rate:F5}{FromAccountCurrency?.Name}, 1{FromAccountCurrency?.Name}={d:F5}{ToAccountCurrency?.Name}";
                }
                return "N/A";
            }
        }

        public AccountFilterModel ToAccount
        {
            get => toAccount;
            set
            {
                toAccount = value;
                RaisePropertyChanged(nameof(ToAccount));
                RaisePropertyChanged(nameof(RateString));
                RaisePropertyChanged(nameof(IsToAmountVisible));
                RaisePropertyChanged(nameof(ToAccountCurrency));
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

        public CurrencyModel ToAccountCurrency
        {
            get => DbManual.Currencies.FirstOrDefault(x => x.Id == (ToAccount != null ? ToAccount.CurrencyId : 0));
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
