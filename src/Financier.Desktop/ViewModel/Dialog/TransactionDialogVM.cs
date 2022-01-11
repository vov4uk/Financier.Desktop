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

        private DelegateCommand<TransactionDto> _deleteSubTransactionCommand;

        private DelegateCommand<TransactionDto> _openSubTransactionDialogCommand;

            TransactionDto transaction,

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
                    ShowSubTransactionDialog(new TransactionDto(), true);
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

        public DelegateCommand<TransactionDto> DeleteSubTransactionCommand
        {
            get
            {
                return _deleteSubTransactionCommand ??= new DelegateCommand<TransactionDto>(tr =>
                {
                    Transaction.SubTransactions.Remove(tr);
                    Transaction.RecalculateUnSplitAmount();
                });
            }
        }

        public DelegateCommand<TransactionDto> EditSubTransactionCommand
        {
            get
            {
                return _openSubTransactionDialogCommand ??=
                    new DelegateCommand<TransactionDto>(tr => ShowSubTransactionDialog(tr, false));
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
            tr.Note = modifiedCopy.Note;
            tr.ProjectId = modifiedCopy.ProjectId;
        }

        private void CopySubTransaction(TransactionDto original, TransactionDto modifiedCopy)
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

            if (output is List<TransactionDto>)
            copy.RequestSave += (sender, _) =>
                var outputTransactions = output as List<TransactionDto>;
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

        private void ShowSubTransactionDialog(TransactionDto original, bool isNewItem)
        {
            var dialog = new WizardWindow();
            var workingCopy = new TransactionDto { IsSubTransaction = true };
            var viewModel = new RecipesVM(Categories.ToList(), Projects.ToList()) { TotalAmount = Transaction.FromAmount / 100.0 };
            viewModel.CreatePages();
            viewModel.RequestClose += (o, args) =>
            {
                dialog.Close();
                if (args)
                {
                    var vm = o as RecipesVM;
                    foreach (var item in vm.TransactionsToImport)
            if (dialogResult is TransactionDto)
                    {
                var modifiedCopy = dialogResult as TransactionDto;
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