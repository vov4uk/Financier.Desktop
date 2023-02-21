namespace Financier.Desktop.Tests.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Financier.Adapter;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.DataAccess.Abstractions;
    using Financier.DataAccess.Data;
    using Financier.DataAccess.View;
    using Financier.Desktop.Data;
    using Financier.Desktop.Helpers;
    using Financier.Desktop.ViewModel;
    using Financier.Desktop.ViewModel.Dialog;
    using Financier.Desktop.Views;
    using Financier.Desktop.Views.Controls;
    using Financier.Desktop.Wizards;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Moq;
    using Xunit;
    using Path = System.IO.Path;

    public class FinancierVMTest
    {
        private readonly Mock<IDialogWrapper> dialogMock;
        private readonly Mock<IFinancierDatabaseFactory> dbFactoryMock;
        private readonly Mock<IFinancierDatabase> dbMock;
        private readonly Mock<IBankHelper> csvMock;
        private readonly Mock<IBankHelperFactory> bankMock;
        private readonly Mock<IUnitOfWork> uowMock;
        private readonly Mock<IEntityReader> entityReaderMock;
        private readonly Mock<IBackupWriter> backupWriterMock;
        private readonly Mock<IBaseRepository<Account>> accountsRepo;
        private readonly Mock<IBaseRepository<BlotterTransactions>> transactionsRepo;
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
            this.bankMock = new (MockBehavior.Strict);
            this.csvMock = new (MockBehavior.Strict);
            this.dialogMock = new(MockBehavior.Strict);
            this.dbFactoryMock = new(MockBehavior.Strict);
            this.dbMock = new(MockBehavior.Strict);
            this.uowMock = new(MockBehavior.Strict);
            this.entityReaderMock = new(MockBehavior.Strict);
            this.backupWriterMock = new(MockBehavior.Strict);
            this.accountsRepo = new ();
            this.transactionsRepo = new ();
            this.categoriesRepo = new ();
            this.exchangeRatesRepo = new ();

            this.dbFactoryMock.Setup(x => x.CreateDatabase())
                .Returns(this.dbMock.Object);
            this.bankMock.Setup(x => x.CreateBankHelper(It.IsAny<string>()))
                .Returns(this.csvMock.Object);
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
        public async Task MenuNavigateCommand_ChangeCurrentPage_PropertiesUpdated()
        {
            await this.SetupDbManual();
            var vm = this.GetFinancierVM();

            this.locMock = new Mock<IBaseRepository<Location>>();
            this.payeeMock = new Mock<IBaseRepository<Payee>>();
            this.projMock = new Mock<IBaseRepository<Project>>();

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.GetRepository<BlotterTransactions>()).Returns(transactionsRepo.Object);
            this.uowMock.Setup(x => x.Dispose());

            this.SetupRepo(this.exchangeRatesRepo);
            this.SetupRepo(this.locMock);
            this.SetupRepo(this.projMock);
            this.SetupRepo(this.payeeMock);

            await vm.MenuNavigateCommand.ExecuteAsync(typeof(BlotterModel));
            Assert.True(vm.IsTransactionPageSelected);
            await vm.MenuNavigateCommand.ExecuteAsync(typeof(LocationModel));
            Assert.True(vm.IsLocationPageSelected);
            await vm.MenuNavigateCommand.ExecuteAsync(typeof(ProjectModel));
            Assert.True(vm.IsProjectPageSelected);
            await vm.MenuNavigateCommand.ExecuteAsync(typeof(PayeeModel));
            Assert.True(vm.IsPayeePageSelected);
            await vm.MenuNavigateCommand.ExecuteAsync(typeof(ExchangeRateModel));
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
            await this.SetupDbManual();
            var vm = this.GetFinancierVM();
            this.entityReaderMock.Setup(x => x.ParseBackupFile(backupPath)).Returns((entities, backupVersion, entityColumnsOrder));

            this.dialogMock.Setup(x => x.ShowMessageBox(It.IsAny<string>(), "Success", false)).Returns(true);
            this.dbMock.Setup(x => x.ImportEntitiesAsync(entities)).Returns(Task.CompletedTask).Verifiable();
            this.dbMock.Setup(x => x.Dispose());
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose()).Verifiable();
            this.SetupRepo(this.transactionsRepo);

            await vm.OpenBackup(backupPath);

            this.dbMock.Verify();

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
            this.budgetMock = new ();
            this.trAMock = new ();
            this.currMock = new ();
            this.locMock = new ();
            this.payeeMock = new ();
            this.projMock = new ();
            this.trMock = new ();
            this.adMock = new ();
            this.caMock = new ();
            this.cccdMock = new ();
            this.smsMock = new ();

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
        public async void SaveBackupAsDb_ExecuteCommand_DBQueryExecuted(string backupPath)
        {
            var vm = this.GetFinancierVM();
            this.dbMock.Setup(x => x.SaveAsFile(backupPath)).Returns(Task.CompletedTask).Verifiable();
            this.dialogMock.Setup(x => x.SaveFileDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(backupPath).Verifiable();
            this.dialogMock.Setup(x => x.ShowMessageBox($"Saved {backupPath}", "Backup done.", false)).Returns(true).Verifiable();

            await vm.SaveBackupAsDbCommand.ExecuteAsync();

            this.dbMock.VerifyAll();
            this.dialogMock.VerifyAll();
        }

        [Theory]
        [AutoMoqData]
        public async Task Locations_AddRaised_NewItemAdded(LocationDto result)
        {
            await this.SetupDbManual();
            var location = new Location() { Id = 0 };
            Location[] actual = null;

            this.dbMock.Setup(x => x.GetOrCreateAsync<Location>(0)).ReturnsAsync(location);

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<Location[]>())).Callback<IEnumerable<Location>>((x) => { actual = x.ToArray(); }).Returns(Task.CompletedTask);
            this.dialogMock.Setup(x => x.ShowDialog<LocationControl>(It.IsAny<LocationControlVM>(), 240, 300, nameof(Location))).Returns(result);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose()).Verifiable();
            this.locMock = new Mock<IBaseRepository<Location>>();
            this.SetupRepo(this.locMock);

            var vm = this.GetFinancierVM();
            await vm.Locations.AddCommand.ExecuteAsync();

            this.dbMock.Verify();
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
        public async Task Locations_AddRaisedCancelClicked_NoNewItemAdded()
        {
            var location = new Location() { Id = 0 };

            this.dbMock.Setup(x => x.GetOrCreateAsync<Location>(0)).ReturnsAsync(location);

            this.dialogMock.Setup(x => x.ShowDialog<LocationControl>(It.IsAny<LocationControlVM>(), 240, 300, nameof(Location))).Returns(null);

            this.locMock = new ();
            this.SetupRepo(this.locMock);

            var vm = this.GetFinancierVM();
            await vm.Locations.AddCommand.ExecuteAsync();

            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.GetOrCreateAsync<Location>(0), Times.Once);
            this.dbMock.Verify(x => x.InsertOrUpdateAsync<Location>(It.IsAny<Location[]>()), Times.Never);
        }

        [Theory]
        [AutoMoqData]
        public async Task Projects_AddRaised_NewItemAdded(TagDto result)
        {
            var location = new Project() { Id = 0 };
            Project[] actual = null;

            this.dbMock.Setup(x => x.GetOrCreateAsync<Project>(0)).ReturnsAsync(location);
            await this.SetupDbManual();
            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<Project[]>())).Callback<IEnumerable<Project>>((x) => { actual = x.ToArray(); }).Returns(Task.CompletedTask);
            this.dialogMock.Setup(x => x.ShowDialog<TagControl>(It.IsAny<TagControlVM>(), 180, 300, nameof(Project))).Returns(result);

            this.projMock = new ();
            this.SetupRepo(this.projMock);

            var vm = this.GetFinancierVM();
            await vm.Projects.AddCommand.ExecuteAsync();

            this.dbMock.Verify(x => x.GetOrCreateAsync<Project>(0), Times.Once);
            this.dbMock.Verify(x => x.InsertOrUpdateAsync(It.IsAny<Project[]>()), Times.Once);
            Assert.Equal(result.IsActive, actual[0].IsActive);
            Assert.Equal(result.Title, actual[0].Title);
            Assert.Equal(0, actual[0].Id);
        }

        [Fact]
        public async Task Projects_AddRaisedCancelClicked_NoNewItemAdded()
        {
            var location = new Project() { Id = 0 };

            this.dbMock.Setup(x => x.GetOrCreateAsync<Project>(0)).ReturnsAsync(location);

            this.dialogMock.Setup(x => x.ShowDialog<TagControl>(It.IsAny<TagControlVM>(), 180, 300, nameof(Project))).Returns(null);

            this.projMock = new ();
            this.SetupRepo(this.projMock);

            var vm = this.GetFinancierVM();
            await vm.Projects.AddCommand.ExecuteAsync();

            this.dbMock.VerifyAll();
            this.dbMock.Verify(x => x.GetOrCreateAsync<Project>(0), Times.Once);
            this.dbMock.Verify(x => x.InsertOrUpdateAsync(It.IsAny<Project[]>()), Times.Never);
        }

        [Fact]
        public async Task MonoCommand_OpenWizard_AddNewTransaction()
        {
            await this.SetupDbManual();

            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            this.dialogMock.Setup(x => x.OpenFileDialog("csv")).Returns(csvPath);
            this.dialogMock.Setup(x => x.ShowMessageBox("Imported 1 transactions.", "Monobank Import", false))
                .Returns(true);

            SetupImportWizard(csvPath);

            var vm = this.GetFinancierVM();
            await vm.MonoCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
        }

        [Fact]
        public async Task AbankCommand_OpenWizard_AddNewTransaction()
        {
            await this.SetupDbManual();

            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "abank.pdf");
            this.dialogMock.Setup(x => x.OpenFileDialog("pdf")).Returns(path);
            this.dialogMock.Setup(x => x.ShowMessageBox("Imported 1 transactions.", "A-Bank Import", false))
                .Returns(true);

            SetupImportWizard(path);

            var vm = this.GetFinancierVM();
            await vm.AbankCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
        }

        [Fact]
        public async Task RaiffeisenCommand_OpenWizard_AddNewTransaction()
        {
            await this.SetupDbManual();

            var path = Path.Combine(Environment.CurrentDirectory, "Assets", "raiffaisen.pdf");
            this.dialogMock.Setup(x => x.OpenFileDialog("pdf")).Returns(path);
            this.dialogMock.Setup(x => x.ShowMessageBox("Imported 1 transactions.", "Raiffeisen Import", false))
                .Returns(true);

            SetupImportWizard(path);

            var vm = this.GetFinancierVM();
            await vm.RaiffeisenCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
        }

        [Fact]
        public async Task MonoCommand_DuplicatesFound_NoTransactionsAdded()
        {
            await this.SetupDbManual();
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
            this.dialogMock.Setup(x => x.ShowMessageBox("Imported 0 transactions. Skiped 1 duplicates.", "Monobank Import", false)).Returns(true);

            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new (MockBehavior.Strict);
            this.uowMock.Setup(x => x.GetRepository<Transaction>())
                .Returns(this.trMock.Object);
            this.dbMock.Setup(x => x.AddTransactionsAsync(It.IsAny<List<Transaction>>()))
                .Returns(Task.CompletedTask);
            this.trMock.Setup(x => x.FindManyAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
                .ReturnsAsync(findManyOutput);
            this.dbMock.Setup(x => x.CreateUnitOfWork())
                .Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.csvMock.Setup(x => x.ParseReport(csvPath))
                .ReturnsAsync(Array.Empty<BankTransaction>());

            var vm = this.GetFinancierVM();
            await vm.MonoCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
        }

        [Fact]
        public async Task MonoCommand_Cancel_NoTransactionsAdded()
        {
            var csvPath = Path.Combine(Environment.CurrentDirectory, "Assets", "mono.ukr.csv");
            this.dialogMock.Setup(x => x.OpenFileDialog("csv")).Returns(csvPath);
            this.dialogMock.Setup(x => x.ShowWizard(It.IsAny<MonoWizardVM>())).Returns(null);
            this.dialogMock.Setup(x => x.ShowMessageBox("Imported 0 transactions. Skiped 1 duplicates.", "Monobank CSV Import", false))
                .Returns(true);
            this.SetupWizardRepos();

            this.csvMock.Setup(x => x.ParseReport(csvPath)).ReturnsAsync(Array.Empty<BankTransaction>());

            var vm = this.GetFinancierVM();
            await vm.MonoCommand.ExecuteAsync();

            this.dbMock.VerifyAll();
        }

        [Theory]
        [AutoMoqData]
        public async Task OpenTransaction_ExistingTransaction_UpdateTransaction(
            BlotterModel eventArgs,
            Transaction transaction,
            AccountFilterModel account,
            IEnumerable<Transaction> subTransactions)
        {
            eventArgs.CategoryId = -1;
            await this.SetupDbManual();
            var output = new TransactionDto(transaction, subTransactions);
            output.FromAccount = account;

            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<Payee>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new (MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs.Id))
                .ReturnsAsync(transaction);
            this.dbMock.Setup(x => x.GetOrCreateAsync<Transaction>(It.IsAny<int>()))
                .ReturnsAsync(new Transaction());
            this.dbMock.Setup(x => x.GetSubTransactionsAsync(It.IsAny<int>()))
                .ReturnsAsync(Array.Empty<Transaction>());

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<List<Transaction>>()))
                .Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionControlVM>(), 640, 340, nameof(Transaction)))
                .Returns(output);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            await vm.Blotter.EditCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
            this.dbMock.Verify(x => x.GetOrCreateAsync<Transaction>(It.IsAny<int>()), Times.Exactly(subTransactions.Count()));
        }

        [Theory]
        [AutoMoqData]
        public async Task OpenTransaction_NoSubTransaction_UpdateTransaction(
            BlotterModel eventArgs,
            Transaction transaction,
            TransactionDto output)
        {
            eventArgs.CategoryId = -1;
            eventArgs.ToAccountId = 0;

            await this.SetupDbManual();
            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<Payee>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new (MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs.Id))
                .ReturnsAsync(transaction);

            this.dbMock.Setup(x => x.GetSubTransactionsAsync(It.IsAny<int>()))
                .ReturnsAsync(Array.Empty<Transaction>());

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<List<Transaction>>()))
                .Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.CreateUnitOfWork())
                .Returns(this.uowMock.Object);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionControlVM>(), 640, 340, nameof(Transaction)))
                .Returns(output);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            await vm.Blotter.EditCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
        }

        [Theory]
        [AutoMoqData]
        public async Task AddTransaction_NewItem_AddedToRepo(
            Transaction transaction,
            TransactionDto output)
        {
            await this.SetupDbManual();
            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<Payee>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new (MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(0)).ReturnsAsync(transaction);

            this.dbMock.Setup(x => x.GetSubTransactionsAsync(It.IsAny<int>())).ReturnsAsync(Array.Empty<Transaction>());

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<List<Transaction>>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionControlVM>(), 640, 340, nameof(Transaction)))
                .Returns(output);

            var vm = this.GetFinancierVM();
            await vm.Blotter.AddCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
        }

        [Theory]
        [AutoMoqData]
        public async Task OpenTransaction_Cancel_NoUpdateTransaction(BlotterModel eventArgs, Transaction transaction)
        {
            eventArgs.CategoryId = -1;
            await this.SetupDbManual();
            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<Payee>>());
            this.trMock = new (MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs.Id)).ReturnsAsync(transaction);
            this.dbMock.Setup(x => x.GetSubTransactionsAsync(It.IsAny<int>())).ReturnsAsync(Array.Empty<Transaction>());

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransactionControl>(It.IsAny<TransactionControlVM>(), 640, 340, nameof(Transaction)))
                .Returns(null);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            await vm.Blotter.EditCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
        }

        [Theory]
        [AutoMoqData]
        public async Task OpenTransfer_ExistingTransaction_UpdateTransaction(
            BlotterModel eventArgs,
            Transaction transaction,
            TransferDto output)
        {
            eventArgs.FromAccountId = 1;
            eventArgs.ToAccountId = 2;
            eventArgs.CategoryId = 0;

            await this.SetupDbManual();

            this.SetupRepo(new Mock<IBaseRepository<Account>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new (MockBehavior.Strict);
            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs.Id)).ReturnsAsync(transaction);

            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<Transaction[]>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());

            this.dialogMock.Setup(x => x.ShowDialog<TransferControl>(It.IsAny<TransferControlVM>(), 385, 340, "Transfer"))
                .Returns(output);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            await vm.Blotter.EditCommand.ExecuteAsync();

            this.trMock.VerifyAll();
            this.dbMock.Verify();
            this.dbMock.Verify(x => x.RebuildAccountBalanceAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        [Theory]
        [AutoMoqData]
        public async Task AddTransfer_NewTransfer_AddedToDb(
            Transaction transaction,
            TransferDto output)
        {
            await this.SetupDbManual();
            this.SetupRepo(new Mock<IBaseRepository<Account>>());
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());

            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(0)).ReturnsAsync(transaction);
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.InsertOrUpdateAsync(It.IsAny<Transaction[]>())).Returns(Task.CompletedTask);

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransferControl>(It.IsAny<TransferControlVM>(), 385, 340, "Transfer"))
                .Returns(output);

            var vm = this.GetFinancierVM();
            await vm.Blotter.AddTransferCommand.ExecuteAsync();

            this.dbMock.Verify();
            this.dbMock.Verify(x => x.RebuildAccountBalanceAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        [Theory]
        [AutoMoqData]
        public async Task OpenTransfer_Cancel_NoUpdateTransaction(BlotterModel eventArgs, Transaction transaction)
        {
            eventArgs.FromAccountId = 1;
            eventArgs.ToAccountId = 2;
            eventArgs.CategoryId = 0;

            this.SetupRepo(new Mock<IBaseRepository<Account>>());

            this.dbMock.Setup(x => x.GetOrCreateTransactionAsync(eventArgs.Id)).ReturnsAsync(transaction);

            this.uowMock.Setup(x => x.Dispose());
            this.dialogMock.Setup(x => x.ShowDialog<TransferControl>(It.IsAny<TransferControlVM>(), 385, 340, "Transfer"))
                .Returns(null);

            var vm = this.GetFinancierVM();
            vm.Blotter.SelectedValue = eventArgs;
            await vm.Blotter.EditCommand.ExecuteAsync();

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

        private MainWindowVM GetFinancierVM() => new MainWindowVM(this.dialogMock.Object, this.dbFactoryMock.Object, this.entityReaderMock.Object, this.backupWriterMock.Object, this.bankMock.Object);

        private void SetupRepo<T>(Mock<IBaseRepository<T>> mock)
            where T : Entity
        {
            this.uowMock.Setup(x => x.GetRepository<T>()).Returns(mock.Object);
            mock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<T, object>>[]>())).ReturnsAsync(new List<T>());
        }

        private async Task SetupDbManual()
        {
            this.dbMock.Setup(x => x.ExecuteQuery<AccountFilterModel>(It.IsAny<string>())).ReturnsAsync(new List<AccountFilterModel>() { new AccountFilterModel() });
            this.dbMock.Setup(x => x.ExecuteQuery<CategoryModel>(It.IsAny<string>())).ReturnsAsync(new List<CategoryModel>() { new CategoryModel() });
            this.dbMock.Setup(x => x.ExecuteQuery<CurrencyModel>(It.IsAny<string>())).ReturnsAsync(new List<CurrencyModel>() { new CurrencyModel() });
            this.dbMock.Setup(x => x.ExecuteQuery<PayeeModel>(It.IsAny<string>())).ReturnsAsync(new List<PayeeModel>() { new PayeeModel() });
            this.dbMock.Setup(x => x.ExecuteQuery<ProjectModel>(It.IsAny<string>())).ReturnsAsync(new List<ProjectModel>() { new ProjectModel() });
            this.dbMock.Setup(x => x.ExecuteQuery<YearMonths>(It.IsAny<string>())).ReturnsAsync(new List<YearMonths>() { new YearMonths() });
            this.dbMock.Setup(x => x.ExecuteQuery<Years>(It.IsAny<string>())).ReturnsAsync(new List<Years>() { new Years() });
            this.dbMock.Setup(x => x.ExecuteQuery<LocationModel>(It.IsAny<string>())).ReturnsAsync(new List<LocationModel>() { new LocationModel() });

            DbManual.ResetAllManuals();
            await DbManual.SetupAsync(this.dbMock.Object);
        }

        private void SetupImportWizard(string path)
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

            this.dialogMock.Setup(x => x.ShowWizard(It.IsAny<MonoWizardVM>()))
                .Returns(output);
            this.SetupWizardRepos();
            this.SetupRepo(new Mock<IBaseRepository<BlotterTransactions>>());
            this.trMock = new(MockBehavior.Strict);
            this.uowMock.Setup(x => x.GetRepository<Transaction>()).Returns(this.trMock.Object);
            this.dbMock.Setup(x => x.AddTransactionsAsync(It.IsAny<List<Transaction>>())).Returns(Task.CompletedTask);
            this.trMock.Setup(x => x.FindManyAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(new List<Transaction>());
            this.dbMock.Setup(x => x.RebuildAccountBalanceAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.Dispose());
            this.csvMock.Setup(x => x.ParseReport(path)).ReturnsAsync(Array.Empty<BankTransaction>());
        }
    }
}
