using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Financier.DataAccess.Data;
using Financier.Desktop.Entities;
using Prism.Commands;
using Prism.Mvvm;

namespace Financier.Desktop.ViewModel
{
    public class TransactionVM : BindableBase
    {
        private DelegateCommand _addSubTransactionCommand;

        private DelegateCommand _cancelCommand;

        private DelegateCommand _changeFromAmountSignCommand;

        private DelegateCommand<int?> _clearCategoryCommand;

        private DelegateCommand _clearFromAmountCommand;

        private DelegateCommand _clearLocationCommand;

        private DelegateCommand _clearNotesCommand;

        private DelegateCommand _clearOriginalFromAmountCommand;

        private DelegateCommand _clearPayeeCommand;

        private DelegateCommand _clearProjectCommand;

        private DelegateCommand<TransactionVM> _deleteSubTransactionCommand;

        private DelegateCommand<TransactionVM> _openSubTransactionDialogCommand;

        private DelegateCommand _saveCommand;

        private Account account;

        private int accountId;

        private Category category;

        private int? categoryId;

        private Currency currency;

        private int? currencyId;

        // TODO : Split SubTransaction entities as base class
        private DateTime date;
        private long fromAmount;
        private int id;
        private bool isAmountNegative;
        private int? locationId;
        private string note;
        private long? originalFromAmount;
        private long parentTransactionSplitAmount;
        private int? payeeId;
        private int? projectId;
        private double rate;
        private ObservableCollection<TransactionVM> subTransactions;
        private long unSplitAmount;
        public event EventHandler RequestCancel;

        public event EventHandler RequestSave;

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

        public ObservableCollection<Account> Accounts { get; set; }

        public DelegateCommand AddSubTransactionCommand
        {
            get
            {
                return _addSubTransactionCommand ??= new DelegateCommand(() =>
                    ShowSubTransactionDialog(
                        new TransactionVM { FromAmount = UnsplitAmount, IsAmountNegative = UnsplitAmount < 0 }, true));
            }
        }

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ??= new DelegateCommand(() => RequestCancel?.Invoke(this, EventArgs.Empty)); }
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

        public DelegateCommand ChangeFromAmountSignCommand
        {
            get
            {
                return _changeFromAmountSignCommand ??=
                    new DelegateCommand(() => { IsAmountNegative = !isAmountNegative; });
            }
        }

        public DelegateCommand<int?> ClearCategoryCommand
        {
            get
            {
                return _clearCategoryCommand ??= new DelegateCommand<int?>(
                    i => { CategoryId = i; });
            }
        }

        public DelegateCommand ClearFromAmountCommand
        {
            get { return _clearFromAmountCommand ??= new DelegateCommand(() => { FromAmount = 0; }); }
        }

        public DelegateCommand ClearLocationCommand
        {
            get { return _clearLocationCommand ??= new DelegateCommand(() => { LocationId = default; }); }
        }

        public DelegateCommand ClearNotesCommand
        {
            get { return _clearNotesCommand ??= new DelegateCommand(() => { Note = default; }); }
        }

        public DelegateCommand ClearOriginalFromAmountCommand
        {
            get { return _clearOriginalFromAmountCommand ??= new DelegateCommand(() => { OriginalFromAmount = 0; }); }
        }

        public DelegateCommand ClearPayeeCommand
        {
            get { return _clearPayeeCommand ??= new DelegateCommand(() => { PayeeId = default; }); }
        }

        public DelegateCommand ClearProjectCommand
        {
            get { return _clearProjectCommand ??= new DelegateCommand(() => { projectId = default; }); }
        }

        public ObservableCollection<Currency> Currencies { get; set; }

        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        public DelegateCommand<TransactionVM> DeleteSubTransactionCommand
        {
            get
            {
                return _deleteSubTransactionCommand ??= new DelegateCommand<TransactionVM>(tr =>
                {
                    subTransactions.Remove(tr);
                    RecalculateUnSplitAmount();
                });
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

        public int Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
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

        public ObservableCollection<Location> Locations { get; set; }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                RaisePropertyChanged(nameof(Note));
            }
        }

        public DelegateCommand<TransactionVM> OpenSubTransactionDialogCommand
        {
            get
            {
                return _openSubTransactionDialogCommand ??=
                    new DelegateCommand<TransactionVM>(tr => ShowSubTransactionDialog(tr, false));
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

        public ObservableCollection<Payee> Payees { get; set; }

        #region SubTransaction entities

        public ObservableCollection<Category> Categories { get; set; }

        public ObservableCollection<Project> Projects { get; set; }

        #endregion
        public int? ProjectId
        {
            get => projectId;
            set
            {
                projectId = value;
                RaisePropertyChanged(nameof(ProjectId));
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
                    return
                        $"1{currency?.Name}={Rate:F5}{account?.Currency.Name}, 1{account?.Currency?.Name}={d:F5}{currency?.Name}";
                }

                return "N/A";
            }
        }

        public long RealFromAmount => Math.Abs(FromAmount) * (IsAmountNegative ? -1 : 1);

        public DelegateCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new DelegateCommand(() => RequestSave?.Invoke(this, EventArgs.Empty),
                    CanSaveCommandExecute);
            }
        }

        public long SplitAmount
        {
            get { return subTransactions?.Sum(x => x.fromAmount) ?? 0; }
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

        public long UnsplitAmount
        {
            get => unSplitAmount;
            set
            {
                unSplitAmount = value;
                RaisePropertyChanged(nameof(UnsplitAmount));
            }
        }

        private bool CanSaveCommandExecute()
        {
            if (IsSplitCategory) return UnsplitAmount == 0;
            return true;
        }

        private void CopySubTransaction(TransactionVM tr, TransactionVM modifiedCopy)
        {
            tr.CategoryId = modifiedCopy.CategoryId;
            tr.Category = Categories.FirstOrDefault(x => x.Id == modifiedCopy.CategoryId);
            tr.FromAmount = modifiedCopy.RealFromAmount;
            tr.Note = modifiedCopy.Note;
            tr.ProjectId = modifiedCopy.ProjectId;
        }

        private void RecalculateRate()
        {
            if (originalFromAmount != null && originalFromAmount != 0)
                Rate = Math.Abs(fromAmount / 100.0 / (originalFromAmount.Value / 100.0));
        }

        private void RecalculateUnSplitAmount()
        {
            if (!IsSubTransaction)
                UnsplitAmount = RealFromAmount - SplitAmount;
            else
                UnsplitAmount = ParentTransactionUnSplitAmount - RealFromAmount;
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
                ShowInTaskbar = Debugger.IsAttached
            };

            copy.RequestCancel += (_, _) => { dialog.Close(); };
            copy.RequestSave += (sender, _) =>
            {
                dialog.Close();
                var modifiedCopy = sender as TransactionVM;
                CopySubTransaction(tr, modifiedCopy);

                if (isNewItem) subTransactions.Add(tr);
                RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            };
            dialog.Content = new SubTransactionControl {DataContext = copy};
            dialog.ShowDialog();
        }
    }
}