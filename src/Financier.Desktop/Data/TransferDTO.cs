using Financier.DataAccess.Data;
using Financier.Converters;
using System;
using Financier.Common.Model;
using Financier.Common.Entities;
using System.Linq;
using Financier.Common.Utils;

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

        public TransferDto() { }

        public TransferDto(Transaction transaction)
        {
            id = transaction.Id;
            fromAccountId = transaction.FromAccountId;
            toAccountId = transaction.ToAccountId;
            note = transaction.Note;
            fromAmount = transaction.FromAmount;
            toAmount = transaction.ToAmount;
            date = UnixTimeConverter.Convert(transaction.DateTime).Date;
            time = UnixTimeConverter.Convert(transaction.DateTime);
            fromAccount = DbManual.Account.FirstOrDefault(x => x.Id == fromAccountId);
            toAccount = DbManual.Account.FirstOrDefault(x => x.Id == toAccountId);
        }

        public AccountFilterModel FromAccount
        {
            get => fromAccount;
            set
            {
                if (SetProperty(ref fromAccount, value))
                {
                    RaisePropertyChanged(nameof(FromAccount));
                    RaisePropertyChanged(nameof(RateString));
                    RaisePropertyChanged(nameof(IsToAmountVisible));
                    RaisePropertyChanged(nameof(FromAccountCurrency));
                }
            }
        }

        public CurrencyModel FromAccountCurrency
        {
            get => DbManual.Currencies?.FirstOrDefault(x => x.Id == (FromAccount != null ? FromAccount.CurrencyId : 0));
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

        public override long RealFromAmount => -1 * Math.Abs(FromAmount);
        public override bool IsAmountNegative => true;
        public override string SubTransactionTitle => $"{FromAccount?.Title}{BlotterUtils.TRANSFER_DELIMITER}{ToAccount?.Title}";

        public long FromAmount
        {
            get => fromAmount;
            set
            {
                if (SetProperty(ref fromAmount, value))
                {
                    RaisePropertyChanged(nameof(FromAmount));
                    RecalculateRate();
                }
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
                if (SetProperty(ref toAccount, value))
                {
                    RaisePropertyChanged(nameof(ToAccount));
                    RaisePropertyChanged(nameof(RateString));
                    RaisePropertyChanged(nameof(IsToAmountVisible));
                    RaisePropertyChanged(nameof(ToAccountCurrency));
                }
            }
        }

        public int ToAccountId
        {
            get => toAccountId;
            set
            {
                if (SetProperty(ref toAccountId, value))
                {
                    RaisePropertyChanged(nameof(ToAccountId));
                }
            }
        }

        public CurrencyModel ToAccountCurrency
        {
            get => DbManual.Currencies?.FirstOrDefault(x => x.Id == (ToAccount != null ? ToAccount.CurrencyId : 0));
        }

        public long ToAmount
        {
            get => toAmount;
            set
            {
                if (SetProperty(ref toAmount, value))
                {
                    RaisePropertyChanged(nameof(ToAmount));
                    RecalculateRate();
                }
            }
        }

        internal void RecalculateRate()
        {
            if (fromAmount != 0 && toAmount != 0)
            {
                Rate = Math.Abs(fromAmount / 100.0 / (toAmount / 100.0));
            }
        }
    }
}
