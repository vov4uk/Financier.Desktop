﻿using System;
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
        private DelegateCommand _clearLocationCommand;
        private DelegateCommand _clearPayeeCommand;
        private DelegateCommand<TransactionDto> _deleteSubTransactionCommand;
        private DelegateCommand _openRecipesDialogCommand;
        private DelegateCommand<TransactionDto> _openSubTransactionDialogCommand;

        public TransactionControlVM(
            TransactionDto transaction,
            IDialogWrapper dialogWrapper)
            :base(transaction)
        {
            this.dialogWrapper = dialogWrapper;
        }

        public DelegateCommand AddSubTransactionCommand => _addSubTransactionCommand ??= new DelegateCommand(() => { ShowSubTransactionDialog(new TransactionDto(), true); });

        public DelegateCommand ClearLocationCommand => _clearLocationCommand ??= new DelegateCommand(() => { Transaction.LocationId = default; });

        public DelegateCommand ClearPayeeCommand => _clearPayeeCommand ??= new DelegateCommand(() => { Transaction.PayeeId = default; });

        public DelegateCommand<TransactionDto> DeleteSubTransactionCommand => _deleteSubTransactionCommand ??= new DelegateCommand<TransactionDto>(tr =>
                                                                                            {
                                                                                                Transaction.SubTransactions.Remove(tr);
                                                                                                Transaction.RecalculateUnSplitAmount();
                                                                                            });

        public DelegateCommand<TransactionDto> EditSubTransactionCommand => _openSubTransactionDialogCommand ??= new DelegateCommand<TransactionDto>(tr => ShowSubTransactionDialog(tr, false));

        public DelegateCommand OpenRecipesDialogCommand => _openRecipesDialogCommand ??= new DelegateCommand(ShowRecepiesDialog);

        protected override bool CanSaveCommandExecute() => Transaction.Account != null && Transaction.FromAmount != 0;

        private static void CopySubTransaction(TransactionDto original, TransactionDto modifiedCopy)
        {
            original.CategoryId = modifiedCopy.CategoryId;
            original.Category = DbManual.Category?.FirstOrDefault(x => x.Id == modifiedCopy.CategoryId);
            original.FromAmount = modifiedCopy.RealFromAmount;
            original.IsAmountNegative = modifiedCopy.IsAmountNegative;
            original.Note = modifiedCopy.Note;
            original.ProjectId = modifiedCopy.ProjectId;
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
                    item.Category = DbManual.Category?.FirstOrDefault(x => x.Id == item.CategoryId);
                    Transaction.SubTransactions.Add(item);
                }
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