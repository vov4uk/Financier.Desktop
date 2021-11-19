using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Financier.DataAccess.Data;
using Financier.Desktop.Views;
using Financier.Desktop.Wizards;
using Financier.Desktop.Wizards.RecipesWizard.ViewModel;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransactionDialogVM : SubTransactionDailogVM
    {
        private DelegateCommand _addSubTransactionCommand;

        private DelegateCommand _clearLocationCommand;

        private DelegateCommand _clearPayeeCommand;

        private DelegateCommand<TransactionDTO> _deleteSubTransactionCommand;

        private DelegateCommand<TransactionDTO> _openSubTransactionDialogCommand;

        private DelegateCommand _openRecipesDialogCommand;

        public ObservableCollection<Account> Accounts { get; set; }

        public ObservableCollection<Currency> Currencies { get; set; }

        public ObservableCollection<Location> Locations { get; set; }

        public ObservableCollection<Payee> Payees { get; set; }

        public DelegateCommand AddSubTransactionCommand
        {
            get
            {
                return _addSubTransactionCommand ??= new DelegateCommand(() =>
                {
                    var transaction = new TransactionDTO
                    {
                        FromAmount = Transaction.UnsplitAmount,
                        IsAmountNegative = Transaction.UnsplitAmount < 0
                    };
                    ShowSubTransactionDialog(transaction, true);
                });
            }
        }

        public DelegateCommand ClearLocationCommand
        {
            get { return _clearLocationCommand ??= new DelegateCommand(() => { Transaction.LocationId = default; }); }
        }

        public DelegateCommand ClearPayeeCommand
        {
            get { return _clearPayeeCommand ??= new DelegateCommand(() => { Transaction.PayeeId = default; }); }
        }

        public DelegateCommand<TransactionDTO> DeleteSubTransactionCommand
        {
            get
            {
                return _deleteSubTransactionCommand ??= new DelegateCommand<TransactionDTO>(tr =>
                {
                    Transaction.SubTransactions.Remove(tr);
                    Transaction.RecalculateUnSplitAmount();
                });
            }
        }

        public DelegateCommand<TransactionDTO> OpenSubTransactionDialogCommand
        {
            get
            {
                return _openSubTransactionDialogCommand ??=
                    new DelegateCommand<TransactionDTO>(tr => ShowSubTransactionDialog(tr, false));
            }
        }

        public DelegateCommand OpenRecipesDialogCommand
        {
            get
            {
                return _openRecipesDialogCommand ??=
                    new DelegateCommand(ShowRecepiesDialog);
            }
        }

        private void CopySubTransaction(TransactionDTO tr, TransactionDTO modifiedCopy)
        {
            tr.CategoryId = modifiedCopy.CategoryId;
            tr.Category = Categories.FirstOrDefault(x => x.Id == modifiedCopy.CategoryId);
            tr.FromAmount = modifiedCopy.RealFromAmount;
            tr.IsAmountNegative = modifiedCopy.IsAmountNegative;
            tr.Note = modifiedCopy.Note;
            tr.ProjectId = modifiedCopy.ProjectId;
        }

        private void ShowSubTransactionDialog(TransactionDTO tr, bool isNewItem)
        {
            var copy = new SubTransactionDailogVM() { Transaction = new TransactionDTO() };
            CopySubTransaction(copy.Transaction, tr);

            copy.Transaction.IsAmountNegative = tr.FromAmount <= 0;
            copy.Transaction.FromAmount = Math.Abs(tr.FromAmount);
            copy.Categories = Categories;
            copy.Projects = Projects;
            copy.Transaction.IsSubTransaction = true;
            Transaction.RecalculateUnSplitAmount();
            copy.Transaction.ParentTransactionUnSplitAmount = isNewItem ? Transaction.UnsplitAmount : Transaction.UnsplitAmount - Math.Abs(tr.FromAmount);
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
                var modifiedCopy = sender as SubTransactionDailogVM;
                CopySubTransaction(tr, modifiedCopy.Transaction);

                if (isNewItem) Transaction.SubTransactions.Add(tr);
                Transaction.RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            };
            dialog.Content = new SubTransactionControl { DataContext = copy };
            dialog.ShowDialog();
        }

        private void ShowRecepiesDialog()
        {
            var dialog = new WizardWindow();

            var viewModel = new RecipesVM(Transaction.RealFromAmount / 100.0, Categories.Where(x => x.Id > 0).ToList(), Projects.OrderByDescending(x => x.IsActive).ThenBy(x => x.Id).ToList());

            viewModel.RequestClose += (o, args) =>
            {
                dialog.Close();
                if (args)
                {
                    var vm = o as RecipesVM;
                    foreach (var item in vm.TransactionsToImport)
                    {
                        item.Category = Categories.FirstOrDefault(x => x.Id == item.CategoryId);
                        Transaction.SubTransactions.Add(item);
                    }
                    Transaction.RecalculateUnSplitAmount();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            };
            dialog.DataContext = viewModel;
            dialog.ShowDialog();
        }
    }
}