using Financier.DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransferDialogVM : BindableBase
    {
        private DelegateCommand _cancelCommand;
        private DelegateCommand _clearNotesCommand;
        private DelegateCommand _saveCommand;
        private DateTime date;
        private Account fromAccount;
        private int fromAccountId;
        private long fromAmount;
        private int id;
        private string note;
        private double rate;
        private Account toAccount;
        private int toAccountId;
        private long toAmount;
        public event EventHandler RequestCancel;

        public event EventHandler RequestSave;

        public ObservableCollection<Account> Accounts { get; set; }
        public DelegateCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??= new DelegateCommand(() => RequestCancel?.Invoke(this, EventArgs.Empty));
            }
        }

        public DelegateCommand ClearNotesCommand
        {
            get
            {
                return _clearNotesCommand ??= new DelegateCommand(() => { Note = default; });
            }
        }

        public DelegateCommand SaveCommand
        {
            get { return _saveCommand ??= new DelegateCommand(() => RequestSave?.Invoke(this, EventArgs.Empty)); }
        }

        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

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
                if (fromAmount != 0 && toAmount != 0)
                {
                    Rate = Math.Abs(fromAmount / 100.0 / (toAmount / 100.0));
                }
            }
        }

        public int Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }

        public bool IsToAmountVisible => fromAccount != null && toAccount != null && fromAccount.CurrencyId != toAccount.CurrencyId;

        public string Note
        {
            get => note;
            set
            {
                note = value;
                RaisePropertyChanged(nameof(Note));
            }
        }

        public double Rate
        {
            get => rate;
            set
            {
                rate = value;
                RaisePropertyChanged(nameof(Rate));
                RaisePropertyChanged(nameof(RateString));
            }
        }

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
                if (fromAmount != 0 && toAmount != 0)
                {
                    Rate = Math.Abs(fromAmount / 100.0 / (toAmount / 100.0));
                }
            }
        }
    }
}
