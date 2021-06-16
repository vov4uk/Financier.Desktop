using Financier.DataAccess.Data;
using Financier.Desktop.Entities;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Financier.Desktop.ViewModel
{
    public class TransactionVM : BindableBase
    {
        // TODO : Split SubTransaction entities as base class
        private DateTime date;
        private int id;
        private int accountId;
        private int? payeeId;
        private int? currencyId;
        private int? categoryId;
        private long fromAmount;
        private long? originalFromAmmount;
        private long unSplitAmount;
        private long parentTransactionSplitedAmount = 0;
        private int? locationId;
        private string note;
        private int? projectId;
        private bool isAmountNegative;
        private ObservableCollection<TransactionVM> subTransactions;
        private Category category;
        private Currency currency;
        private Account account;
        private double rate;

        public ObservableCollection<Account> Accounts { get; set; }

        public ObservableCollection<Currency> Currencies { get; set; }

        public ObservableCollection<Location> Locations { get; set; }

        public ObservableCollection<Payee> Payees { get; set; }

        #region SubTransaction entities
        public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<Project> Projects { get; set; }
        #endregion

        public event EventHandler RequestCancel;
        public event EventHandler RequestSave;

        private DelegateCommand _changeFromAmountSignCommand;
        public DelegateCommand ChangeFromAmountSignCommand
        {
            get
            {
                return _changeFromAmountSignCommand ??= new DelegateCommand(() => { IsAmountNegative = !isAmountNegative; });
            }
        }

        private DelegateCommand _clearPayeeCommand;
        public DelegateCommand ClearPayeeCommand
        {
            get
            {
                return _clearPayeeCommand ??= new DelegateCommand(() => { PayeeId = default; });
            }
        }

        private DelegateCommand _clearNotesCommand;
        public DelegateCommand ClearNotesCommand
        {
            get
            {
                return _clearNotesCommand ??= new DelegateCommand(() => { Note = default; });
            }
        }

        private DelegateCommand _clearLocationCommand;
        public DelegateCommand ClearLocationCommand
        {
            get
            {
                return _clearLocationCommand ??= new DelegateCommand(() => { LocationId = default; });
            }
        }

        private DelegateCommand<int?> _clearCategoryCommand;
        public DelegateCommand<int?> ClearCategoryCommand
        {
            get
            {
                return _clearCategoryCommand ??= new DelegateCommand<int?>(
                        (int? i) =>
                        {
                            CategoryId = i;
                        });
            }
        }

        private DelegateCommand _clearFromAmountCommand;
        public DelegateCommand ClearFromAmountCommand
        {
            get
            {
                return _clearFromAmountCommand ??= new DelegateCommand(() => { FromAmount = 0; });
            }
        }

        private DelegateCommand _clearOriginalFromAmountCommand;
        public DelegateCommand ClearOriginalFromAmountCommand
        {
            get
            {
                return _clearOriginalFromAmountCommand ??= new DelegateCommand(() => { OriginalFromAmount = 0; });
            }
        }

        private DelegateCommand _clearProjectCommand;
        public DelegateCommand ClearProjectCommand
        {
            get
            {
                return _clearProjectCommand ??= new DelegateCommand(() => { projectId = default; });
            }
        }

        private DelegateCommand<TransactionVM> _deleteSubTransactionCommand;
        public DelegateCommand<TransactionVM> DeleteSubTransactionCommand
        {
            get
            {
                return _deleteSubTransactionCommand ??= new DelegateCommand<TransactionVM>((TransactionVM tr) =>
                {
                    subTransactions.Remove(tr);
                    RecalculateUnSplitAmount();
                });
            }
        }

        private DelegateCommand<TransactionVM> _openSubTransactionDialogCommand;
        public DelegateCommand<TransactionVM> OpenSubTransactionDialogCommand
        {
            get
            {
                return _openSubTransactionDialogCommand ??= new DelegateCommand<TransactionVM>((tr) => ShowSubTransactionDialog(tr, false));
            }
        }

        private DelegateCommand _addSubTransactionCommand;
        public DelegateCommand AddSubTransactionCommand
        {
            get
            {
                return _addSubTransactionCommand ??= new DelegateCommand(() => ShowSubTransactionDialog(new TransactionVM() { FromAmount = UnsplitAmount , IsAmountNegative = UnsplitAmount < 0 }, true));
            }
        }

        private void ShowSubTransactionDialog(TransactionVM tr, bool isNewItem)
        {
            var copy = new TransactionVM();
            CopySubTransaction(copy, tr);

            copy.IsAmountNegative = tr.FromAmount <= 0;
            copy.FromAmount = Math.Abs(tr.FromAmount);
            copy.Categories = Categories;
            copy.Projects = Projects;
            copy.IsSubTransaction = true;
            RecalculateUnSplitAmount();
            copy.ParentTransactionUnSplitAmount = isNewItem ? UnsplitAmount : UnsplitAmount - Math.Abs(tr.FromAmount);
            var dialog = new Window
            {
                ResizeMode = ResizeMode.NoResize,
                Height = 340,
                Width = 340,
                ShowInTaskbar = Debugger.IsAttached,
            };

            copy.RequestCancel += (sender, args) =>
            {
                dialog.Close();
            };
            copy.RequestSave += (sender, args) =>
            {
                dialog.Close();
                var modifiedCopy = sender as TransactionVM;
                CopySubTransaction(tr, modifiedCopy);

                if (isNewItem)
                {
                    subTransactions.Add(tr);
                }
                RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            };
            dialog.Content = new SubTransactionControl() { DataContext = copy };
            dialog.ShowDialog();
        }

        private void CopySubTransaction(TransactionVM tr, TransactionVM modifiedCopy)
        {
            tr.CategoryId = modifiedCopy.CategoryId;
            tr.Category = Categories.FirstOrDefault(x => x.Id == modifiedCopy.CategoryId);
            tr.FromAmount = modifiedCopy.RealFromAmount;
            tr.Note = modifiedCopy.Note;
            tr.ProjectId = modifiedCopy.ProjectId;
        }

        private DelegateCommand _cancelCommand;
        public DelegateCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??= new DelegateCommand(() => RequestCancel?.Invoke(this, EventArgs.Empty));
            }
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new DelegateCommand(() => RequestSave?.Invoke(this, EventArgs.Empty), CanSaveCommandExecute);
            }
        }

        private bool CanSaveCommandExecute()
        {
            if (IsSplitCategory)
            {
                return UnsplitAmount == 0;
            }
            return true;
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

        public Currency Currency
        {
            get => currency;
            set
            {
                currency = value;
                RaisePropertyChanged(nameof(Currency));
                RaisePropertyChanged(nameof(IsOriginalFromAmountVisible));
                RaisePropertyChanged(nameof(RateString));
            }
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

        public ObservableCollection<TransactionVM> SubTransactions
        {
            get => subTransactions;
            set
            {
                subTransactions = value;
                RaisePropertyChanged(nameof(SubTransactions));
                RecalculateUnSplitAmount();
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

        public int AccountId
        {
            get => accountId;
            set
            {
                accountId = value;
                RaisePropertyChanged(nameof(AccountId));
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

        public int? PayeeId
        {
            get => payeeId;
            set
            {
                payeeId = value;
                RaisePropertyChanged(nameof(PayeeId));
            }
        }

        public int? CurrencyId
        {
            get => currencyId;
            set
            {
                currencyId = value;
                RaisePropertyChanged(nameof(CurrencyId));
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

        public bool IsSplitCategory
        {
            get { return categoryId == -1; }
        }
        
        public long RealFromAmount
        {
            get { return Math.Abs(FromAmount) * (IsAmountNegative ? -1 : 1); }
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

        public long ParentTransactionUnSplitAmount
        {
            get => parentTransactionSplitedAmount;
            set
            {
                parentTransactionSplitedAmount = value;
                RaisePropertyChanged(nameof(ParentTransactionUnSplitAmount));
                RecalculateUnSplitAmount();
            }
        }

        public long SplitAmount
        {
            get
            {
                return subTransactions?.Sum(x => x.fromAmount) ?? 0;
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
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsSubTransaction { get; set; }

        private void RecalculateRate()
        {
            if (originalFromAmmount != null && originalFromAmmount != 0)
            {
                Rate = Math.Abs((fromAmount / 100.0) / (originalFromAmmount.Value / 100.0));
            }
        }

        private void RecalculateUnSplitAmount()
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

        public long? OriginalFromAmount
        {
            get => originalFromAmmount;
            set
            {
                originalFromAmmount = value;
                RaisePropertyChanged(nameof(OriginalFromAmount));
                RecalculateRate();
            }
        }

        public bool IsOriginalFromAmountVisible
        {
            get { return currency != null && account != null && currency?.Id != account?.Currency.Id; }
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
                    return $"1{this.currency?.Name}={Rate:F5}{account?.Currency.Name}, 1{account?.Currency?.Name}={rate:F5}{currency?.Name}";
                }
                return "N/A";
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

        public int? LocationId
        {
            get => locationId;
            set
            {
                locationId = value;
                RaisePropertyChanged(nameof(LocationId));
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

        public int? ProjectId
        {
            get => projectId;
            set
            {
                projectId = value;
                RaisePropertyChanged(nameof(ProjectId));
            }
        }
    }
}
