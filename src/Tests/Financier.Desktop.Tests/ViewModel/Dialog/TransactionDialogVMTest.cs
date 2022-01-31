namespace Financier.Desktop.Tests.ViewModel.Dialog
{
    using System;
    using System.Collections.Generic;
    using Financier.DataAccess.Data;
    using Financier.Desktop.Data;
    using Financier.Desktop.Helpers;
    using Financier.Desktop.ViewModel.Dialog;
    using Financier.Desktop.Views;
    using Financier.Desktop.Wizards;
    using Financier.Desktop.Wizards.RecipesWizard.ViewModel;
    using Financier.Tests.Common;
    using Moq;
    using Xunit;

    public class TransactionDialogVMTest
    {
        private readonly Mock<IDialogWrapper> dialogMock;

        public TransactionDialogVMTest()
        {
            this.dialogMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
        }

        [Theory]
        [AutoMoqData]
        public void Constructor_WithParameters_CollectionsHaveValues(
            TransactionDto transaction,
            List<Category> categories,
            List<Project> projects,
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Payee> payees)
        {
            var vm = new TransactionDialogVM(
                transaction,
                this.dialogMock.Object,
                categories,
                projects,
                accounts,
                currencies,
                locations,
                payees);

            Assert.NotEmpty(vm.Categories);
            Assert.NotEmpty(vm.Projects);
            Assert.NotEmpty(vm.Accounts);
            Assert.NotEmpty(vm.Currencies);
            Assert.NotEmpty(vm.Locations);
            Assert.NotEmpty(vm.Payees);
        }

        [Theory]
        [AutoMoqData]
        public void AddSubTransactionCommand_Execute_TransactionAdded(
            TransactionDto transaction,
            TransactionDto subTransaction,
            List<Category> categories,
            List<Project> projects,
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Payee> payees)
        {
            TransactionDto workingCopy = default;
            transaction.SubTransactions.Clear();
            subTransaction.FromAmount = 100;
            transaction.FromAmount = 100;
            transaction.IsAmountNegative = true;
            transaction.IsSubTransaction = false;
            transaction.OriginalCurrencyId = 0;
            transaction.OriginalCurrency = default;
            subTransaction.OriginalCurrencyId = 0;
            subTransaction.OriginalCurrency = default;

            this.dialogMock.Setup(x => x.ShowDialog<SubTransactionControl>(It.IsAny<SubTransactionDailogVM>(), 340, 340, "Sub Transaction"))
                .Callback<DialogBaseVM, double, double, string>((a, _, _, _) => { workingCopy = ((SubTransactionDailogVM)a).Transaction; })
                .Returns(subTransaction);

            var vm = new TransactionDialogVM(
                transaction,
                this.dialogMock.Object,
                categories,
                projects,
                accounts,
                currencies,
                locations,
                payees);

            vm.AddSubTransactionCommand.Execute();

            Assert.Single(vm.Transaction.SubTransactions);
            Assert.Equal(0, vm.Transaction.UnsplitAmount);
            Assert.True(workingCopy.IsSubTransaction);
            Assert.True(workingCopy.IsAmountNegative);
            Assert.Equal(100, workingCopy.FromAmount);
            Assert.Equal(-100, workingCopy.ParentTransactionUnSplitAmount);
            Assert.True(vm.SaveCommand.CanExecute());
        }

        [Theory]
        [AutoMoqData]
        public void OpenSubTransactionDialogCommand_Execute_TransactionUpdated(
            TransactionDto transaction,
            TransactionDto subTransaction,
            List<Category> categories,
            List<Project> projects,
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Payee> payees)
        {
            TransactionDto workingCopy = default;
            transaction.SubTransactions.Clear();
            transaction.FromAmount = 100;
            transaction.IsAmountNegative = true;
            transaction.IsSubTransaction = false;
            transaction.OriginalCurrencyId = 0;
            transaction.OriginalCurrency = default;

            subTransaction.OriginalCurrencyId = 0;
            subTransaction.OriginalCurrency = default;
            subTransaction.IsSubTransaction = true;

            this.dialogMock.Setup(x => x.ShowDialog<SubTransactionControl>(It.IsAny<SubTransactionDailogVM>(), 340, 340, "Sub Transaction"))
                .Callback<DialogBaseVM, double, double, string>((a, _, _, _) => { workingCopy = ((SubTransactionDailogVM)a).Transaction; })
                .Returns(subTransaction);

            var vm = new TransactionDialogVM(
                transaction,
                this.dialogMock.Object,
                categories,
                projects,
                accounts,
                currencies,
                locations,
                payees);

            vm.EditSubTransactionCommand.Execute(subTransaction);

            Assert.Empty(vm.Transaction.SubTransactions); // no new items added to collection
            Assert.True(workingCopy.IsSubTransaction);
            Assert.Equal(subTransaction.RealFromAmount < 0, workingCopy.IsAmountNegative);
            Assert.Equal(Math.Abs(subTransaction.FromAmount), workingCopy.FromAmount);
        }

        [Theory]
        [AutoMoqData]
        public void OpenRecipesDialogCommand_Execute_TransactionsAdded(
            TransactionDto transaction,
            List<TransactionDto> outputTransactions,
            List<Category> categories,
            List<Project> projects,
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Payee> payees)
        {
            transaction.FromAmount = 1000;
            transaction.IsAmountNegative = true;
            transaction.SubTransactions.Clear();
            transaction.OriginalCurrencyId = 0;
            transaction.OriginalCurrency = default;

            RecipesVM recipesVM = default;

            this.dialogMock.Setup(x => x.ShowWizard(It.IsAny<RecipesVM>())).Callback<WizardBaseVM>(x => recipesVM = (RecipesVM)x)
                .Returns(outputTransactions);

            var vm = new TransactionDialogVM(
                transaction,
                this.dialogMock.Object,
                categories,
                projects,
                accounts,
                currencies,
                locations,
                payees);

            vm.OpenRecipesDialogCommand.Execute();

            Assert.Equal(-10, recipesVM.TotalAmount);
            Assert.Equal(outputTransactions.Count, vm.Transaction.SubTransactions.Count);
        }

        [Theory]
        [AutoMoqData]
        public void ClearCommand_Execute_SetDefaultValues(
            TransactionDto transaction,
            List<Category> categories,
            List<Project> projects,
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Payee> payees)
        {
            var vm = new TransactionDialogVM(
                transaction,
                this.dialogMock.Object,
                categories,
                projects,
                accounts,
                currencies,
                locations,
                payees);

            vm.ClearLocationCommand.Execute();
            vm.ClearNotesCommand.Execute();
            vm.ClearPayeeCommand.Execute();
            vm.ClearProjectCommand.Execute();
            vm.ClearFromAmountCommand.Execute();
            vm.ClearOriginalFromAmountCommand.Execute();

            Assert.Null(vm.Transaction.LocationId);
            Assert.Null(vm.Transaction.Note);
            Assert.Null(vm.Transaction.PayeeId);
            Assert.Null(vm.Transaction.ProjectId);
            Assert.Equal(0, vm.Transaction.FromAmount);
            Assert.Equal(0, vm.Transaction.OriginalFromAmount);
        }
    }
}
