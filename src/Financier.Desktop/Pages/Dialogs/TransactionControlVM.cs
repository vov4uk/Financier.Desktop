using System;
using System.Collections.Generic;
using System.Linq;
using Financier.Common.Entities;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Views;
using Financier.Desktop.Wizards.RecipesWizard.ViewModel;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class TransactionControlVM : SubTransactionControlVM
    {
        private readonly IDialogWrapper dialogWrapper;
        private DelegateCommand _addSubTransactionCommand;
        private DelegateCommand _addSubTransferCommand;
        private DelegateCommand _clearLocationCommand;
        private DelegateCommand _clearPayeeCommand;
        private DelegateCommand<BaseTransactionDto> _deleteSubTransactionCommand;
        private DelegateCommand _openRecipesDialogCommand;
        private DelegateCommand<BaseTransactionDto> _editSubTransaction;

        public TransactionControlVM(
            TransactionDto transaction,
            IDialogWrapper dialogWrapper)
            :base(transaction)
        {
            this.dialogWrapper = dialogWrapper;
        }

        public DelegateCommand AddSubTransactionCommand => _addSubTransactionCommand ??= new DelegateCommand(() => { ShowSubTransactionDialog(new TransactionDto(), true); });
        public DelegateCommand AddSubTransferCommand => _addSubTransferCommand ??= new DelegateCommand(() => { ShowSubTransferDialog(new TransferDto(), true); });

        public DelegateCommand ClearLocationCommand => _clearLocationCommand ??= new DelegateCommand(() => { Transaction.LocationId = default; });

        public DelegateCommand ClearPayeeCommand => _clearPayeeCommand ??= new DelegateCommand(() => { Transaction.PayeeId = default; });

        public DelegateCommand<BaseTransactionDto> DeleteSubTransactionCommand => _deleteSubTransactionCommand ??= new DelegateCommand<BaseTransactionDto>(tr =>
                                                                                            {
                                                                                                Transaction.SubTransactions.Remove(tr);
                                                                                                Transaction.RecalculateUnSplitAmount();
                                                                                            });

        public DelegateCommand<BaseTransactionDto> EditSubTransactionCommand => _editSubTransaction ??= new DelegateCommand<BaseTransactionDto>(EditSubTransaction);

        public DelegateCommand OpenRecipesDialogCommand => _openRecipesDialogCommand ??= new DelegateCommand(ShowRecepiesDialog);

        protected override bool CanSaveCommandExecute() => Transaction.FromAccount != null && Transaction.FromAmount != 0;

        private static void CopySubTransaction(TransactionDto original, TransactionDto modifiedCopy)
        {
            original.CategoryId = modifiedCopy.CategoryId;
            original.Category = DbManual.Category?.Find(x => x.Id == modifiedCopy.CategoryId);
            original.FromAmount = modifiedCopy.RealFromAmount;
            original.IsAmountNegative = modifiedCopy.IsAmountNegative;
            original.Note = modifiedCopy.Note;
            original.ProjectId = modifiedCopy.ProjectId;
        }

        private static void CopySubTransfer(TransferDto original, TransferDto modifiedCopy)
        {
            original.Id = modifiedCopy.Id;
            original.FromAccountId = modifiedCopy.FromAccountId;
            original.FromAccount = modifiedCopy.FromAccount;
            original.ToAccountId = modifiedCopy.ToAccountId;
            original.ToAccount = modifiedCopy.ToAccount;
            original.Note = modifiedCopy.Note;
            original.FromAmount = modifiedCopy.RealFromAmount;
            original.ToAmount = Math.Abs(modifiedCopy.FromAmount);
            original.Date =modifiedCopy.DateTime.Date;
            original.Time = modifiedCopy.DateTime;
        }

        private void ShowRecepiesDialog()
        {
            var vm = new RecipesVM(Transaction.RealFromAmount / 100.0);

            var output = dialogWrapper.ShowWizard(vm);

            var outputTransactions = output as List<TransactionDto>;
            if (outputTransactions != null)
            {
                foreach (var item in outputTransactions)
                {
                    item.Category = DbManual.Category?.Find(x => x.Id == item.CategoryId);
                    Transaction.SubTransactions.Add(item);
                }
                Transaction.RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        private void EditSubTransaction(BaseTransactionDto original)
        {
            if (original is TransactionDto)
            {
                ShowSubTransactionDialog(original as TransactionDto, false);
            }
            else if (original is TransferDto)
            {
                ShowSubTransferDialog(original as TransferDto, false);
            }
        }

        private void ShowSubTransferDialog(TransferDto original, bool isNewItem)
        {
            if (Transaction.IsOriginalFromAmountVisible)
            {
                this.dialogWrapper.ShowMessageBox("Split-transfers with a currency differ from account's currency are not yet supported", "Not supported");
                return;
            }

            Transaction.RecalculateUnSplitAmount();
            var workingCopy = new TransferDto()
            {
                FromAccountId = Transaction.FromAccountId,
                IsSubTransaction = true,
                FromAmount = Transaction.UnsplitAmount,
                Date = Transaction.Date,
                Time = Transaction.Time,
            };
            if (!isNewItem)
            {
                CopySubTransfer(workingCopy, original);
            }

            var viewModel = new TransferControlVM(workingCopy);

            var dialogResult = dialogWrapper.ShowDialog<TransferControl>(viewModel, 385, 340, "Transfer");

            var modifiedCopy = dialogResult as TransferDto;
            if (modifiedCopy != null)
            {
                CopySubTransfer(original, modifiedCopy);

                if (isNewItem) Transaction.SubTransactions.Add(original);
                Transaction.RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void ShowSubTransactionDialog(TransactionDto original, bool isNewItem)
        {
            Transaction.RecalculateUnSplitAmount();
            var workingCopy = new TransactionDto { IsSubTransaction = true };
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

            var viewModel = new SubTransactionControlVM(workingCopy);

            var dialogResult = dialogWrapper.ShowDialog<SubTransactionControl>(viewModel, 340, 340, "Sub Transaction");

            var modifiedCopy = dialogResult as TransactionDto;
            if (modifiedCopy != null)
            {
                CopySubTransaction(original, modifiedCopy);

                if (isNewItem) Transaction.SubTransactions.Add(original);
                Transaction.RecalculateUnSplitAmount();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}