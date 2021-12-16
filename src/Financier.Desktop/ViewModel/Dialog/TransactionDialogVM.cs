using System;
using System.Collections.ObjectModel;
using System.Linq;
using Financier.DataAccess.Data;
using Financier.Desktop.Helpers;
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

        private void CopySubTransaction(TransactionDTO original, TransactionDTO modifiedCopy)
        {
            original.CategoryId = modifiedCopy.CategoryId;
            original.Category = Categories.FirstOrDefault(x => x.Id == modifiedCopy.CategoryId);
            original.FromAmount = modifiedCopy.RealFromAmount;
            original.IsAmountNegative = modifiedCopy.IsAmountNegative;
            original.Note = modifiedCopy.Note;
            original.ProjectId = modifiedCopy.ProjectId;
        }

        private void ShowSubTransactionDialog(TransactionDTO dto, bool isNewItem)
        {
            var copy = new SubTransactionDailogVM() { Transaction = new TransactionDTO() };
            CopySubTransaction(copy.Transaction, dto);

            copy.Transaction.IsAmountNegative = dto.FromAmount <= 0;
            copy.Transaction.FromAmount = Math.Abs(dto.FromAmount);
            copy.Categories = Categories;
            copy.Projects = Projects;
            copy.Transaction.IsSubTransaction = true;

            Transaction.RecalculateUnSplitAmount();

            copy.Transaction.ParentTransactionUnSplitAmount = isNewItem ? Transaction.UnsplitAmount : Transaction.UnsplitAmount - Math.Abs(dto.FromAmount);

            var result = DialogHelper.ShowDialog<SubTransactionControl>(copy, 340, 340, "Sub Transaction");

            if (result is SubTransactionDailogVM)
            {
                var modifiedCopy = result as SubTransactionDailogVM;
                CopySubTransaction(dto, modifiedCopy.Transaction);

                if (isNewItem) Transaction.SubTransactions.Add(dto);
                Transaction.RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void ShowRecepiesDialog()
        {
            var dialog = new WizardWindow();

            var viewModel = new RecipesVM(
                Transaction.RealFromAmount / 100.0,
                Categories.Where(x => x.Id > 0).ToList(),
                Projects.ToList());

            var save = DialogHelper.ShowWizard(viewModel);

            if (save)
            {
                foreach (var item in viewModel.TransactionsToImport)
                {
                    item.Category = Categories.FirstOrDefault(x => x.Id == item.CategoryId);
                    Transaction.SubTransactions.Add(item);
                }
                Transaction.RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}