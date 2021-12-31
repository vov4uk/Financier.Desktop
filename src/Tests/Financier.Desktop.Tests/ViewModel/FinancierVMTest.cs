namespace Financier.Desktop.Tests.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Financier.Adapter;
    using Financier.DataAccess.Abstractions;
    using Financier.DataAccess.Data;
    using Financier.DataAccess.Monobank;
    using Financier.DataAccess.View;
    using Financier.Desktop.Data;
    using Financier.Desktop.Helpers;
    using Financier.Desktop.ViewModel;
    using Financier.Desktop.ViewModel.Dialog;
    using Financier.Desktop.Views;
    using Financier.Desktop.Views.Controls;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Moq;
    using Xunit;

    public class FinancierVMTest
    {
        private readonly Mock<IDialogWrapper> dialogMock;
        private readonly Mock<IFinancierDatabaseFactory> dbFactoryMock;
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly Mock<ICsvHelper> csvMock;
        private readonly Mock<IUnitOfWork> uowMock;
        private readonly Mock<IEntityReader> entityReaderMock;
        private readonly Mock<IBackupWriter> backupWriterMock;
        private readonly Mock<IBaseRepository<Account>> accountsRepo;
        private readonly Mock<IBaseRepository<BlotterTransactions>> transactionsRepo;
        private readonly Mock<IBaseRepository<ByCategoryReport>> categoryReportsRepo;
        private readonly Mock<IBaseRepository<Category>> categoriesRepo;
        private readonly Mock<IBaseRepository<CurrencyExchangeRate>> exchangeRatesRepo;

        private Mock<IBaseRepository<Budget>> budgetMock;
        private Mock<IBaseRepository<TransactionAttribute>> trAMock;
        private Mock<IBaseRepository<Currency>> currMock;
        private Mock<IBaseRepository<Location>> locMock;
        private Mock<IBaseRepository<Payee>> payeeMock;
        private Mock<IBaseRepository<Project>> projMock;
        private Mock<IBaseRepository<Transaction>> trMock;
        private Mock<IBaseRepository<AttributeDefinition>> adMock;
        private Mock<IBaseRepository<CategoryAttribute>> caMock;
        private Mock<IBaseRepository<CCardClosingDate>> cccdMock;
        private Mock<IBaseRepository<SmsTemplate>> smsMock;

        public FinancierVMTest()
        {
            this.csvMock = new Mock<ICsvHelper>(MockBehavior.Strict);
            this.dialogMock = new Mock<IDialogWrapper>(MockBehavior.Strict);
            this.dbFactoryMock = new Mock<IFinancierDatabaseFactory>(MockBehavior.Strict);
            this.dbMock = new Mock<IFinancierDatabase>(MockBehavior.Strict);
            this.uowMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
            this.entityReaderMock = new Mock<IEntityReader>(MockBehavior.Strict);
            this.backupWriterMock = new Mock<IBackupWriter>(MockBehavior.Strict);
            this.accountsRepo = new Mock<IBaseRepository<Account>>();
            this.transactionsRepo = new Mock<IBaseRepository<BlotterTransactions>>();
            this.categoryReportsRepo = new Mock<IBaseRepository<ByCategoryReport>>();
            this.categoriesRepo = new Mock<IBaseRepository<Category>>();
            this.exchangeRatesRepo = new Mock<IBaseRepository<CurrencyExchangeRate>>();

            this.dbFactoryMock.Setup(x => x.CreateDatabase()).Returns(this.dbMock.Object);
        }

        [Fact]
        public void Constructor_NoParameters_PagesCreated()
        {
            var vm = this.GetFinancierVM();

            Assert.NotNull(vm.Blotter);
            Assert.NotNull(vm.Locations);
            Assert.NotNull(vm.Payees);
            Assert.NotNull(vm.Projects);
        }

        [Fact]
        public void MenuNavigateCommand_ChangeCurrentPage_PropertiesUpdated()
        {
            var vm = this.GetFinancierVM();

            this.locMock = new Mock<IBaseRepository<Location>>();
            this.payeeMock = new Mock<IBaseRepository<Payee>>();
            this.projMock = new Mock<IBaseRepository<Project>>();

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());

            this.SetupRepo(this.exchangeRatesRepo);
            this.SetupRepo(this.locMock);
            this.SetupRepo(this.projMock);
            this.SetupRepo(this.payeeMock);

            vm.MenuNavigateCommand.Execute(typeof(BlotterTransactions));
            Assert.True(vm.IsTransactionPageSelected);
            vm.MenuNavigateCommand.Execute(typeof(Location));
            Assert.True(vm.IsLocationPageSelected);
            vm.MenuNavigateCommand.Execute(typeof(Project));
            Assert.True(vm.IsProjectPageSelected);
            vm.MenuNavigateCommand.Execute(typeof(Payee));
            Assert.True(vm.IsPayeePageSelected);
            vm.MenuNavigateCommand.Execute(typeof(CurrencyExchangeRate));
            Assert.True(vm.CurrentPage is ExchangeRatesVM);
        }

        [Theory]
        [AutoMoqData]
        public async void OpenBackup_ParseBackup_ImportEntities(
            string backupPath,
            IEnumerable<Entity> entities,
            BackupVersion backupVersion,
            Dictionary<string, List<string>> entityColumnsOrder)
        {
            var vm = this.GetFinancierVM();
            this.entityReaderMock.Setup(x => x.ParseBackupFile(backupPath)).Returns((entities, backupVersion, entityColumnsOrder));

            this.dialogMock.Setup(x => x.ShowMessageBox(It.IsAny<string>(), "Success", false)).Returns(true);
            this.dbMock.Setup(x => x.ImportEntitiesAsync(entities)).Returns(Task.CompletedTask).Verifiable();
            this.dbMock.Setup(x => x.Dispose());
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose()).Verifiable();
            this.SetupRepo(this.transactionsRepo);

            await vm.OpenBackup(backupPath);

            this.dbMock.VerifyAll();

            Assert.True(vm.CurrentPage is BlotterVM);
            Assert.Equal(backupPath, vm.OpenBackupPath);
        }

        [Theory]
        [AutoMoqData]
        public async void SaveBackup_ReadEntitiesFromDb_ExportEntities(
            string backupPath)
        {
            var vm = this.GetFinancierVM();
            this.backupWriterMock.Setup(x => x.GenerateBackup(It.IsAny<List<Entity>>(), backupPath, It.IsAny<BackupVersion>(), It.IsAny<Dictionary<string, List<string>>>(), true)).Verifiable();
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.budgetMock = new Mock<IBaseRepository<Budget>>();
            this.trAMock = new Mock<IBaseRepository<TransactionAttribute>>();
            this.currMock = new Mock<IBaseRepository<Currency>>();
            this.locMock = new Mock<IBaseRepository<Location>>();
            this.payeeMock = new Mock<IBaseRepository<Payee>>();
            this.projMock = new Mock<IBaseRepository<Project>>();
            this.trMock = new Mock<IBaseRepository<Transaction>>();
            this.adMock = new Mock<IBaseRepository<AttributeDefinition>>();
            this.caMock = new Mock<IBaseRepository<CategoryAttribute>>();
            this.cccdMock = new Mock<IBaseRepository<CCardClosingDate>>();
            this.smsMock = new Mock<IBaseRepository<SmsTemplate>>();

            this.SetupRepo(this.accountsRepo);
            this.SetupRepo(this.trMock);
            this.SetupRepo(this.categoriesRepo);
            this.SetupRepo(this.exchangeRatesRepo);
            this.SetupRepo(this.budgetMock);
            this.SetupRepo(this.trAMock);
            this.SetupRepo(this.currMock);
            this.SetupRepo(this.locMock);
            this.SetupRepo(this.payeeMock);
            this.SetupRepo(this.projMock);
            this.SetupRepo(this.adMock);
            this.SetupRepo(this.caMock);
            this.SetupRepo(this.cccdMock);
            this.SetupRepo(this.smsMock);

            this.uowMock.Setup(x => x.Dispose()).Verifiable();

            await vm.SaveBackup(backupPath);

            this.dbMock.VerifyAll();
            this.uowMock.VerifyAll();
            this.entityReaderMock.VerifyAll();
        }

        [Theory]
        [AutoMoqData]
        public void Locations_AddRaised_NewItemAdded(LocationDTO result)
        {
            var location = new Location() { Id = 0 };
            Location[] actual = null;

            this.dbMock.Setup(x => x.GetOrCreateAsync<Location>(0)).ReturnsAsync(location);

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<Location[]>())).Callback<IEnumerable<Location>>((x) => { actual = x.ToArray(); }).Returns(Task.CompletedTask);
            this.dialogMock.Setup(x => x.ShowDialog<LocationControl>(It.IsAny<LocationDialogVM>(), 240, 300, nameof(Location))).Returns(result);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose()).Verifiable();
            this.locMock = new Mock<IBaseRepository<Location>>();
            this.SetupRepo(this.locMock);

            var vm = this.GetFinancierVM();
            vm.Locations.AddCommand.Execute();

            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.GetOrCreateAsync<Location>(0), Times.Once);
            this.dbMock.Verify(x => x.InsertOrUpdateAsync(It.IsAny<Location[]>()), Times.Once);
            Assert.Equal(result.Address, actual[0].Address);
            Assert.Equal(result.IsActive, actual[0].IsActive);
            Assert.Equal(result.Title, actual[0].Title);
            Assert.Equal(result.Title, actual[0].Name);
            Assert.Equal(0, actual[0].Id);
            Assert.Equal(0, actual[0].Count);
        }

        [Fact]
        public void Locations_AddRaisedCancelClicked_NoNewItemAdded()
        {
            var location = new Location() { Id = 0 };

            this.dbMock.Setup(x => x.GetOrCreateAsync<Location>(0)).ReturnsAsync(location);

            this.dialogMock.Setup(x => x.ShowDialog<LocationControl>(It.IsAny<LocationDialogVM>(), 240, 300, nameof(Location))).Returns(null);

            this.locMock = new Mock<IBaseRepository<Location>>();
            this.SetupRepo(this.locMock);

            var vm = this.GetFinancierVM();
            vm.Locations.AddCommand.Execute();

            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.GetOrCreateAsync<Location>(0), Times.Once);
            this.dbMock.Verify(x => x.InsertOrUpdateAsync<Location>(It.IsAny<Location[]>()), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public void Projects_AddRaised_NewItemAdded(EntityWithTitleDTO result)
        {
            var location = new Project() { Id = 0 };
            Project[] actual = null;

            this.dbMock.Setup(x => x.GetOrCreateAsync<Project>(0)).ReturnsAsync(location);

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<Project[]>())).Callback<IEnumerable<Project>>((x) => { actual = x.ToArray(); }).Returns(Task.CompletedTask);
            this.dialogMock.Setup(x => x.ShowDialog<EntityWithTitleControl>(It.IsAny<EntityWithTitleVM>(), 180, 300, nameof(Project))).Returns(result);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose()).Verifiable();
            this.projMock = new Mock<IBaseRepository<Project>>();
            this.SetupRepo(this.projMock);

            var vm = this.GetFinancierVM();
            vm.Projects.AddCommand.Execute();

            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.GetOrCreateAsync<Project>(0), Times.Once);
            this.dbMock.Verify(x => x.InsertOrUpdateAsync(It.IsAny<Project[]>()), Times.Once);
            Assert.Equal(result.IsActive, actual[0].IsActive);
            Assert.Equal(result.Title, actual[0].Title);
            Assert.Equal(0, actual[0].Id);
        }

        [Fact]
        public void Projects_AddRaisedCancelClicked_NoNewItemAdded()
        {
            var location = new Project() { Id = 0 };

            this.dbMock.Setup(x => x.GetOrCreateAsync<Project>(0)).ReturnsAsync(location);

            this.dialogMock.Setup(x => x.ShowDialog<EntityWithTitleControl>(It.IsAny<EntityWithTitleVM>(), 180, 300, nameof(Project))).Returns(null);

            this.projMock = new Mock<IBaseRepository<Project>>();
            this.SetupRepo(this.projMock);

            var vm = this.GetFinancierVM();
            vm.Projects.AddCommand.Execute();

            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.GetOrCreateAsync<Project>(0), Times.Once);
            this.dbMock.Verify(x => x.InsertOrUpdateAsync(It.IsAny<Project[]>()), Times.Never);
        }

        [Fact]
        public void MonoCommand_OpenWizard_AddNewTransaction()
        {
            var output = new List<Transaction>()
                {
                    new Transaction()
                    {
                        Id = 0,
                        FromAmount = 100,
                        CategoryId = 1,
                        OriginalCurrencyId = 0,
                        FromAccountId = 2,
                        OriginalFromAmount = 0,
                        DateTime = 1639121044000,
                    },
                };

            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            this.dialogMock.Setup(x => x.OpenFileDialog("csv")).Returns(csvPath);
            this.dialogMock.Setup(x => x.ShowWizard(It.IsAny<MonoWizardVM>())).Returns(output);
            this.dialogMock.Setup(x => x.ShowMessageBox("Imported 1 transactions.", "Monobank CSV Import", false)).Returns(true);

            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new Mock<IBaseRepository<Transaction>>(MockBehavior.Strict);
            this.uowMock.Setup(x => x.GetRepository<Transaction>()).Returns(this.trMock.Object);
            this.dbMock.Setup(x => x.AddTransactionsAsync(It.IsAny<List<Transaction>>())).Returns(Task.CompletedTask);
            this.trMock.Setup(x => x.FindManyAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(new List<Transaction>());
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.csvMock.Setup(x => x.ParseCsv(csvPath)).ReturnsAsync(Array.Empty<MonoTransaction>());

            var vm = this.GetFinancierVM();
            vm.MonoCommand.Execute();

            this.trMock.VerifyAll();
            this.dbMock.VerifyAll();
        }

        [Fact]
        public void MonoCommand_DuplicatesFound_NoTransactionsAdded()
        {
            var outputTransaction = new Transaction()
            {
                Id = 0,
                FromAmount = 100,
                CategoryId = 1,
                OriginalCurrencyId = 0,
                FromAccountId = 2,
                OriginalFromAmount = 0,
                DateTime = 1639121044000,
            };
            var findManyOutput = new List<Transaction> { outputTransaction };

            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            this.dialogMock.Setup(x => x.OpenFileDialog("csv")).Returns(csvPath);
            this.dialogMock.Setup(x => x.ShowWizard(It.IsAny<MonoWizardVM>())).Returns(findManyOutput);
            this.dialogMock.Setup(x => x.ShowMessageBox("Imported 0 transactions. Skiped 1 duplicates.", "Monobank CSV Import", false)).Returns(true);

            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new Mock<IBaseRepository<Transaction>>(MockBehavior.Strict);
            this.uowMock.Setup(x => x.GetRepository<Transaction>()).Returns(this.trMock.Object);
            this.dbMock.Setup(x => x.AddTransactionsAsync(It.IsAny<List<Transaction>>())).Returns(Task.CompletedTask);
            this.trMock.Setup(x => x.FindManyAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(findManyOutput);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.csvMock.Setup(x => x.ParseCsv(csvPath)).ReturnsAsync(Array.Empty<MonoTransaction>());

            var vm = this.GetFinancierVM();
            vm.MonoCommand.Execute();

            this.trMock.VerifyAll();
            this.dbMock.VerifyAll();
        }

        [Fact]
        public void MonoCommand_Cancel_NoTransactionsAdded()
        {
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            this.dialogMock.Setup(x => x.OpenFileDialog("csv")).Returns(csvPath);
            this.dialogMock.Setup(x => x.ShowWizard(It.IsAny<MonoWizardVM>())).Returns(null);
            this.dialogMock.Setup(x => x.ShowMessageBox("Imported 0 transactions. Skiped 1 duplicates.", "Monobank CSV Import", false))
                .Returns(true);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.SetupWizardRepos();

            this.csvMock.Setup(x => x.ParseCsv(csvPath)).ReturnsAsync(Array.Empty<MonoTransaction>());

            var vm = this.GetFinancierVM();
            vm.MonoCommand.Execute();

            this.dbMock.VerifyAll();
        }

        [Theory]
        [AutoMoqData]
        public void OpenTransaction_ExistingTransaction_UpdateTransaction(
            BlotterTransactions eventArgs,
            Transaction transaction,
            TransactionDTO output,
            ObservableCollection<TransactionDTO> transactionDTOs)
        {
            eventArgs.category_id = -1;
            output.SubTransactions = transactionDTOs;

            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<Payee>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new Mock<IBaseRepository<Transaction>>(MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs._id)).ReturnsAsync(transaction);
            this.dbMock.Setup(x => x.GetOrCreateAsync<Transaction>(It.IsAny<int>())).ReturnsAsync(new Transaction());
            this.dbMock.Setup(x => x.GetSubTransactionsAsync(It.IsAny<int>())).ReturnsAsync(Array.Empty<Transaction>());

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<List<Transaction>>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionDialogVM>(), 640, 340, nameof(Transaction)))
                .Returns(output);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            vm.Blotter.EditCommand.Execute();

            this.trMock.VerifyAll();
            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.GetOrCreateAsync<Transaction>(It.IsAny<int>()), Times.Exactly(transactionDTOs.Count));
        }

        [Theory]
        [AutoMoqData]
        public void OpenTransaction_NoSubTransaction_UpdateTransaction(
            BlotterTransactions eventArgs,
            Transaction transaction,
            TransactionDTO output)
        {
            eventArgs.category_id = -1;

            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<Payee>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new Mock<IBaseRepository<Transaction>>(MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs._id)).ReturnsAsync(transaction);

            this.dbMock.Setup(x => x.GetSubTransactionsAsync(It.IsAny<int>())).ReturnsAsync(Array.Empty<Transaction>());

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<List<Transaction>>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionDialogVM>(), 640, 340, nameof(Transaction)))
                .Returns(output);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            vm.Blotter.EditCommand.Execute();

            this.trMock.VerifyAll();
            this.dbMock.VerifyAll();
        }

        [Theory]
        [AutoMoqData]
        public void AddTransaction_NewItem_AddedToRepo(
            Transaction transaction,
            TransactionDTO output)
        {
            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<Payee>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new Mock<IBaseRepository<Transaction>>(MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(0)).ReturnsAsync(transaction);

            this.dbMock.Setup(x => x.GetSubTransactionsAsync(It.IsAny<int>())).ReturnsAsync(Array.Empty<Transaction>());

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<List<Transaction>>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionDialogVM>(), 640, 340, nameof(Transaction)))
                .Returns(output);

            var vm = this.GetFinancierVM();
            vm.Blotter.AddCommand.Execute();

            this.trMock.VerifyAll();
            this.dbMock.VerifyAll();
        }

        [Theory]
        [AutoMoqData]
        public void OpenTransaction_Cancel_NoUpdateTransaction(BlotterTransactions eventArgs, Transaction transaction)
        {
            eventArgs.category_id = -1;

            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<Payee>>());
            this.trMock = new Mock<IBaseRepository<Transaction>>(MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs._id)).ReturnsAsync(transaction);
            this.dbMock.Setup(x => x.GetSubTransactionsAsync(It.IsAny<int>())).ReturnsAsync(Array.Empty<Transaction>());

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionDialogVM>(), 640, 340, nameof(Transaction)))
                .Returns(null);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            vm.Blotter.EditCommand.Execute();

            this.trMock.VerifyAll();
            this.dbMock.VerifyAll();
        }

        [Theory]
        [AutoMoqData]
        public void OpenTransfer_ExistingTransaction_UpdateTransaction(
            BlotterTransactions eventArgs,
            Transaction transaction,
            TransferDTO output)
        {
            eventArgs.from_account_id = 1;
            eventArgs.to_account_id = 2;
            eventArgs.category_id = 0;

            this.SetupRepo(new Mock<IBaseRepository<Account>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new Mock<IBaseRepository<Transaction>>(MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs._id)).ReturnsAsync(transaction);

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<Transaction[]>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());

            this.dialogMock.Setup(x => x.ShowDialog<TransferControl>(It.IsAny<TransferDialogVM>(), 385, 340, "Transfer"))
                .Returns(output);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            vm.Blotter.EditCommand.Execute();

            this.trMock.VerifyAll();
            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.RebuildAccountBalanceAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        [Theory]
        [AutoMoqData]
        public void AddTransfer_NewTransfer_AddedToDb(
            Transaction transaction,
            TransferDTO output)
        {
            this.SetupRepo(new Mock<IBaseRepository<Account>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());

            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(0)).ReturnsAsync(transaction);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<Transaction[]>())).Returns(Task.CompletedTask);

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransferControl>(It.IsAny<TransferDialogVM>(), 385, 340, "Transfer"))
                .Returns(output);

            var vm = this.GetFinancierVM();
            vm.Blotter.AddTransferCommand.Execute();

            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.RebuildAccountBalanceAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        [Theory]
        [AutoMoqData]
        public void OpenTransfer_Cancel_NoUpdateTransaction(BlotterTransactions eventArgs, Transaction transaction)
        {
            eventArgs.from_account_id = 1;
            eventArgs.to_account_id = 2;
            eventArgs.category_id = 0;

            this.SetupRepo(new Mock<IBaseRepository<Account>>());

            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs._id)).ReturnsAsync(transaction);

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransferControl>(It.IsAny<TransferDialogVM>(), 385, 340, "Transfer"))
                .Returns(null);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            vm.Blotter.EditCommand.Execute();

            this.dbMock.VerifyAll();
        }

        private void SetupWizardRepos()
        {
            this.SetupRepo(new Mock<IBaseRepository<Account>>());
            this.SetupRepo(new Mock<IBaseRepository<Currency>>());
            this.SetupRepo(new Mock<IBaseRepository<Location>>());
            this.SetupRepo(new Mock<IBaseRepository<Category>>());
            this.SetupRepo(new Mock<IBaseRepository<Project>>());
        }

        private FinancierVM GetFinancierVM() => new FinancierVM(this.dialogMock.Object, this.dbFactoryMock.Object, this.entityReaderMock.Object, this.backupWriterMock.Object, this.csvMock.Object);

        private void SetupRepo<T>(Mock<IBaseRepository<T>> mock)
            where T : Entity
        {
            this.uowMock.Setup(x => x.GetRepository<T>()).Returns(mock.Object);
            mock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<T, object>>[]>())).ReturnsAsync(new List<T>());
        }
    }
}
