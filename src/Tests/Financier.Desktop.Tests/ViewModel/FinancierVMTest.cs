namespace Financier.Desktop.Tests.ViewModel
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Financier.Adapter;
    using Financier.DataAccess.Abstractions;
    using Financier.DataAccess.Data;
    using Financier.DataAccess.View;
    using Financier.Desktop.Helpers;
    using Financier.Desktop.Reports.ViewModel;
    using Financier.Desktop.ViewModel;
    using Financier.Tests.Common;
    using Moq;
    using Xunit;

    public class FinancierVMTest
    {
        private readonly Mock<IDialogWrapper> dialogMock;
        private readonly Mock<IFinancierDatabaseFactory> dbFactoryMock;
        private readonly Mock<IFinancierDatabase> dbMock;
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
            Assert.Equal(10, vm.Pages.Count);
        }

        [Fact]
        public void MenuNavigateCommand_ChangeCurrentPage_PropertiesUpdated()
        {
            var vm = this.GetFinancierVM();

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
            Dictionary<string, List<string>> entityColumnsOrder,
            List<Account> accounts,
            List<BlotterTransactions> transactions,
            List<ByCategoryReport> categoryReports,
            List<Category> categories,
            List<CurrencyExchangeRate> exchangeRates)
        {
            var vm = this.GetFinancierVM();
            this.entityReaderMock.Setup(x => x.ParseBackupFile(backupPath)).Returns((entities, backupVersion, entityColumnsOrder));

            this.dbMock.Setup(x => x.Dispose());
            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.GetRepository<Account>()).Returns(this.accountsRepo.Object);
            this.uowMock.Setup(x => x.GetRepository<BlotterTransactions>()).Returns(this.transactionsRepo.Object);
            this.uowMock.Setup(x => x.GetRepository<ByCategoryReport>()).Returns(this.categoryReportsRepo.Object);
            this.uowMock.Setup(x => x.GetRepository<Category>()).Returns(this.categoriesRepo.Object);
            this.uowMock.Setup(x => x.GetRepository<CurrencyExchangeRate>()).Returns(this.exchangeRatesRepo.Object);

            this.dbMock.Setup(x => x.ImportEntitiesAsync(entities)).Returns(Task.CompletedTask).Verifiable();
            this.uowMock.Setup(x => x.Dispose()).Verifiable();
            this.accountsRepo.Setup(x => x.GetAllAsync(x => x.Currency)).ReturnsAsync(accounts);
            this.transactionsRepo.Setup(x => x.GetAllAsync(x => x.from_account_currency, x => x.to_account_currency)).ReturnsAsync(transactions);
            this.categoryReportsRepo.Setup(x => x.GetAllAsync(x => x.from_account_currency, x => x.to_account_currency, x => x.category)).ReturnsAsync(categoryReports);
            this.categoriesRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(categories);
            this.exchangeRatesRepo.Setup(x => x.GetAllAsync(x => x.FromCurrency, x => x.ToCurrency)).ReturnsAsync(exchangeRates);

            await vm.OpenBackup(backupPath);

            this.dbMock.VerifyAll();
            this.uowMock.VerifyAll();
            Assert.Equal(categories.Count, vm.Pages.OfType<CategoriesVM>().First().Entities.Count);
            Assert.Equal(categoryReports.Count, vm.Pages.OfType<ReportVM>().First().Entities.Count);
            Assert.Equal(transactions.Count, vm.Blotter.Entities.Count);
            Assert.Equal(exchangeRates.Count, vm.Pages.OfType<ExchangeRatesVM>().First().Entities.Count);
            Assert.True(vm.CurrentPage is InfoVM);
            Assert.Equal(backupPath, vm.OpenBackupPath);
        }

        [Theory]
        [AutoMoqData]
        public async void SaveBackup_ReadEntitiesFromDb_ExportEntities(
            string backupPath)
        {
            var vm = this.GetFinancierVM();
            this.backupWriterMock.Setup(x => x.GenerateBackup(It.IsAny<List<Entity>>(), backupPath, It.IsAny<BackupVersion>(), It.IsAny<Dictionary<string, List<string>>>(), true)).Verifiable();

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

            this.dbMock.Setup(x => x.CreateUnitOfWork()).Returns(this.uowMock.Object);
            this.uowMock.Setup(x => x.GetRepository<Account>()).Returns(this.accountsRepo.Object);
            this.accountsRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Account>());

            this.uowMock.Setup(x => x.GetRepository<Transaction>()).Returns(this.trMock.Object);
            this.trMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Transaction>());

            this.uowMock.Setup(x => x.GetRepository<Category>()).Returns(this.categoriesRepo.Object);
            this.categoriesRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Category>());

            this.uowMock.Setup(x => x.GetRepository<CurrencyExchangeRate>()).Returns(this.exchangeRatesRepo.Object);
            this.exchangeRatesRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CurrencyExchangeRate>());

            this.uowMock.Setup(x => x.GetRepository<Budget>()).Returns(this.budgetMock.Object);
            this.budgetMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Budget>());

            this.uowMock.Setup(x => x.GetRepository<TransactionAttribute>()).Returns(this.trAMock.Object);
            this.trAMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<TransactionAttribute>());

            this.uowMock.Setup(x => x.GetRepository<Currency>()).Returns(this.currMock.Object);
            this.currMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Currency>());

            this.uowMock.Setup(x => x.GetRepository<Location>()).Returns(this.locMock.Object);
            this.locMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Location>());

            this.uowMock.Setup(x => x.GetRepository<Payee>()).Returns(this.payeeMock.Object);
            this.payeeMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Payee>());

            this.uowMock.Setup(x => x.GetRepository<Project>()).Returns(this.projMock.Object);
            this.projMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Project>());

            this.uowMock.Setup(x => x.GetRepository<AttributeDefinition>()).Returns(this.adMock.Object);
            this.adMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<AttributeDefinition>());

            this.uowMock.Setup(x => x.GetRepository<CategoryAttribute>()).Returns(this.caMock.Object);
            this.caMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CategoryAttribute>());

            this.uowMock.Setup(x => x.GetRepository<CCardClosingDate>()).Returns(this.cccdMock.Object);
            this.cccdMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<CCardClosingDate>());

            this.uowMock.Setup(x => x.GetRepository<SmsTemplate>()).Returns(this.smsMock.Object);
            this.smsMock.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<SmsTemplate>());

            this.uowMock.Setup(x => x.Dispose()).Verifiable();

            await vm.SaveBackup(backupPath);

            this.dbMock.VerifyAll();
            this.uowMock.VerifyAll();
            this.entityReaderMock.VerifyAll();
        }

        private FinancierVM GetFinancierVM() => new FinancierVM(this.dialogMock.Object, this.dbFactoryMock.Object, this.entityReaderMock.Object, this.backupWriterMock.Object);
    }
}
