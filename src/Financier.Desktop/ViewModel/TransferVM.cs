using Financier.DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModel
{
    public class TransferVM : BindableBase, ICloneable
    {
        private DateTime date;
        private int id;
        private int fromAccountId;
        private int toAccountId;
        private long fromAmount;
        private long toAmount;
        private string note;
        private Account fromAccount;
        private Account toAccount;
        private double rate;

        public ObservableCollection<Account> Accounts { get; set; }


        private DelegateCommand _clearNotesCommand;
        public DelegateCommand ClearNotesCommand
        {
            get
            {
                return _clearNotesCommand ??= new DelegateCommand(() => { Note = default; });
            }
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new DelegateCommand(Cancel);

                return _cancelCommand;
            }
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new DelegateCommand(Save);

                return _saveCommand;
            }
        }

        private void Cancel()
        {
            EventHandler handler = RequestCancel;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void Save()
        {
            EventHandler handler = RequestSave;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public event EventHandler RequestCancel;
        public event EventHandler RequestSave;



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


        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                RaisePropertyChanged(nameof(Date));
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

        public int ToAccountId
        {
            get => toAccountId;
            set
            {
                toAccountId = value;
                RaisePropertyChanged(nameof(ToAccountId));
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


        public long FromAmount
        {
            get => fromAmount;
            set
            {
                fromAmount = value;
                RaisePropertyChanged(nameof(FromAmount));
                if (fromAmount != 0 && toAmount != 0)
                {
                    Rate = Math.Abs((fromAmount / 100.0) / (toAmount / 100.0));
                }
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
                    Rate = Math.Abs((fromAmount / 100.0) / (toAmount / 100.0));
                }
            }
        }

        public bool IsToAmountVisible
        {
            get { return fromAccount != null && toAccount != null && fromAccount.CurrencyId != toAccount.CurrencyId; }
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
                    var rate = 1.0 / Rate;
                    return $"1{this.toAccount?.Currency.Name}={Rate:F5}{fromAccount?.Currency.Name}, 1{fromAccount?.Currency?.Name}={rate:F5}{toAccount?.Currency.Name}";
                }
                return "N/A";
            }
        }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                RaisePropertyChanged(nameof(Note));
            }
        }
    }
}
