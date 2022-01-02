using System;
using System.Collections.Generic;
using System.Linq;
using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Views;
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

        private readonly IDialogWrapper dialogWrapper;

        public TransactionDialogVM(
            TransactionDTO transaction,
            IDialogWrapper dialogWrapper,
            List<Category> categories,
            List<Project> projects,
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Payee> payees)
            :base(transaction, categories, projects)
        {
            this.dialogWrapper = dialogWrapper;
            Accounts = accounts;
            Currencies = currencies;
            Locations = locations;
            Payees = payees;
        }
        public List<Account> Accounts { get; }

        public List<Currency> Currencies { get; }

        public List<Location> Locations { get; }

        public List<Payee> Payees { get; }

        public DelegateCommand AddSubTransactionCommand
        {
            get
            {
                return _addSubTransactionCommand ??= new DelegateCommand(() =>
                {
                    ShowSubTransactionDialog(new TransactionDTO(), true);
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

        public DelegateCommand<TransactionDTO> EditSubTransactionCommand
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

        private void ShowSubTransactionDialog(TransactionDTO original, bool isNewItem)
        {
            Transaction.RecalculateUnSplitAmount();
            var workingCopy = new TransactionDTO { IsSubTransaction = true };
            if (isNewItem)
            {
                workingCopy.IsAmountNegative = Transaction.UnsplitAmount < 0;
                workingCopy.FromAmount = Math.Abs(Transaction.UnsplitAmount);
                workingCopy.ParentTransactionUnSplitAmount = Transaction.UnsplitAmount;
            }
            else
            {
                CopySubTransaction(workingCopy, original);
                workingCopy.IsAmountNegative = original.RealFromAmount <= 0;
                workingCopy.FromAmount = Math.Abs(original.FromAmount);
                workingCopy.ParentTransactionUnSplitAmount = Transaction.UnsplitAmount - Math.Abs(original.FromAmount);
            }

            var viewModel = new SubTransactionDailogVM(workingCopy, Categories, Projects);

            var dialogResult = dialogWrapper.ShowDialog<SubTransactionControl>(viewModel, 340, 340, "Sub Transaction");

            if (dialogResult is TransactionDTO)
            {
                var modifiedCopy = dialogResult as TransactionDTO;
                CopySubTransaction(original, modifiedCopy);

                if (isNewItem) Transaction.SubTransactions.Add(original);
                Transaction.RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void ShowRecepiesDialog()
        {
            var vm = new RecipesVM(
                Transaction.RealFromAmount / 100.0,
                Categories.Where(x => x.Id > 0).ToList(),
                Projects.ToList());

            var output = dialogWrapper.ShowWizard(vm);

            if (output is List<TransactionDTO>)
            {
                var outputTransactions = output as List<TransactionDTO>;
                foreach (var item in outputTransactions)
                {
                    item.Category = Categories.FirstOrDefault(x => x.Id == item.CategoryId);
                    Transaction.SubTransactions.Add(item);
                }
                Transaction.RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        protected override bool CanSaveCommandExecute()
        {
            return Transaction.Account != null && Transaction.FromAmount != 0;
        }
    }
}