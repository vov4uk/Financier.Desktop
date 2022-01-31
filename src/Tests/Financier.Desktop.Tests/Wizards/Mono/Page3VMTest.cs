namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Financier.DataAccess.Data;
    using Financier.DataAccess.Monobank;
    using Financier.Desktop.Wizards;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Newtonsoft.Json;
    using Xunit;

    public class Page3VMTest
    {
        [Theory]
        [AutoMoqData]
        public void Constructor_ReceiveParameters_AllPropertiesSeted(
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Category> categories,
            List<Project> projects)
        {
            var vm = new Page3VM(accounts, currencies, locations, categories, projects);
            categories.Insert(0, Category.None);
            Assert.True(vm.Currencies.All(x => currencies.Contains(x)));
            Assert.True(vm.Categories.All(x => categories.Contains(x)));

            Assert.True(vm.Accounts.SequenceEqual(accounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder)));
            Assert.True(vm.Locations.SequenceEqual(locations.OrderByDescending(x => x.IsActive).ThenBy(x => x.Id)));
            Assert.True(vm.Projects.SequenceEqual(projects.OrderByDescending(x => x.IsActive).ThenBy(x => x.Id)));
            Assert.Equal("Please select categories", vm.Title);
            Assert.True(vm.IsValid());
        }

        [Theory]
        [AutoMoqData]
        public void MonoAccount_SetValue_AccountsNotContainsMonoAccount(
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Category> categories,
            List<Project> projects)
        {
            var monoAccount = accounts.FirstOrDefault();

            var vm = new Page3VM(accounts, currencies, locations, categories, projects);

            vm.MonoAccount = monoAccount;

            accounts.Remove(monoAccount);
            Assert.True(vm.Accounts.SequenceEqual(accounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder)));
        }

        [Theory]
        [AutoMoqData]
        public void SetMonoTransactions_SetValue_TransformToFinancierTransactions(
            List<Account> accounts,
            List<Project> projects)
        {
            List<BankTransaction> transactions = new List<BankTransaction>
            {
                new BankTransaction // Description -> Location.Title
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = null,
                    Balance = 1100.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 04),
                    OperationCurrency = "UAH",
                    OperationAmount = 0,
                    Description = "Google",
                    Commission = 0.0,
                    MCC = "1000",
                },
                new BankTransaction// Description -> Location.Address
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = null,
                    Balance = 1200.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 03),
                    OperationCurrency = "UAH",
                    OperationAmount = 0,
                    Description = "Google Store",
                    Commission = 0.0,
                    MCC = "1000",
                },
                new BankTransaction // Description -> Category.Title
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = null,
                    Balance = 1300.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 02),
                    OperationCurrency = "UAH",
                    OperationAmount = 0,
                    Description = "Hot Water",
                    Commission = 0.0,
                    MCC = "1000",
                },
                new BankTransaction // Description -> Note
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = null,
                    Balance = 1400.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 01),
                    OperationCurrency = "UAH",
                    OperationAmount = 0,
                    Description = "Unknown place",
                    Commission = 0.0,
                    MCC = "1000",
                },
                new BankTransaction // OperationCurrency -> OriginalCurrencyId
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = 25.0,
                    Balance = 1500.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 00),
                    OperationCurrency = "USD",
                    OperationAmount = 4.0,
                    Description = "Unknown",
                    Commission = 0.0,
                    MCC = "1000",
                },
            };
            var monoAccount = accounts.FirstOrDefault();
            var expected = new ObservableCollection<FinancierTransactionDto>()
            {
                new FinancierTransactionDto
                {
                    MonoAccountId = monoAccount.Id,
                    FromAmount = 10000,
                    OriginalFromAmount = null,
                    OriginalCurrencyId = 0,
                    CategoryId = 0,
                    ToAccountId = 0,
                    FromAccountId = 0,
                    LocationId = 200,
                    Note = null,
                    DateTime = new DateTimeOffset(new DateTime(2021, 12, 04, 21, 12, 04)).ToUnixTimeMilliseconds(),
                },
                new FinancierTransactionDto
                {
                    MonoAccountId = monoAccount.Id,
                    FromAmount = 10000,
                    OriginalFromAmount = null,
                    OriginalCurrencyId = 0,
                    CategoryId = 0,
                    ToAccountId = 0,
                    FromAccountId = 0,
                    LocationId = 200,
                    Note = null,
                    DateTime = new DateTimeOffset(new DateTime(2021, 12, 04, 21, 12, 03)).ToUnixTimeMilliseconds(),
                },
                new FinancierTransactionDto
                {
                    MonoAccountId = monoAccount.Id,
                    FromAmount = 10000,
                    OriginalFromAmount = null,
                    OriginalCurrencyId = 0,
                    CategoryId = 100,
                    ToAccountId = 0,
                    FromAccountId = 0,
                    LocationId = 0,
                    Note = null,
                    DateTime = new DateTimeOffset(new DateTime(2021, 12, 04, 21, 12, 02)).ToUnixTimeMilliseconds(),
                },
                new FinancierTransactionDto
                {
                    MonoAccountId = monoAccount.Id,
                    FromAmount = 10000,
                    OriginalFromAmount = null,
                    OriginalCurrencyId = 0,
                    CategoryId = 0,
                    ToAccountId = 0,
                    FromAccountId = 0,
                    LocationId = 0,
                    Note = "Unknown place",
                    DateTime = new DateTimeOffset(new DateTime(2021, 12, 04, 21, 12, 01)).ToUnixTimeMilliseconds(),
                },
                new FinancierTransactionDto
                {
                    MonoAccountId = monoAccount.Id,
                    FromAmount = 10000,
                    OriginalFromAmount = 400,
                    OriginalCurrencyId = 1,
                    CategoryId = 0,
                    ToAccountId = 0,
                    FromAccountId = 0,
                    LocationId = 0,
                    Note = "Unknown",
                    DateTime = new DateTimeOffset(new DateTime(2021, 12, 04, 21, 12, 00)).ToUnixTimeMilliseconds(),
                },
            };

            List<Category> categories = new List<Category> { new Category { Title = "Hot water", Id = 100 } };
            List<Location> locations = new List<Location> { new Location { Id = 200, Title = "Google", Address = "Google Store" } };
            List<Currency> currencies = new List<Currency> { new Currency { Id = 1, Name = "USD" }, new Currency { Id = 2, Name = "UAH" } };

            var vm = new Page3VM(accounts, currencies, locations, categories, projects);

            vm.MonoAccount = monoAccount;
            vm.SetMonoTransactions(transactions);

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(vm.FinancierTransactions));
        }

        [Theory]
        [AutoMoqData]
        public void DeleteCommand_Execute_TransactionsRemoved(
            List<Account> accounts,
            List<Currency> currencies,
            List<Location> locations,
            List<Category> categories,
            List<Project> projects)
        {
            List<BankTransaction> transactions = new List<BankTransaction>
            {
                new BankTransaction // Description -> Location.Title
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = null,
                    Balance = 1100.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 04),
                    OperationCurrency = "UAH",
                    OperationAmount = 0,
                    Description = "Google",
                    Commission = 0.0,
                    MCC = "1000",
                },
                new BankTransaction// Description -> Location.Address
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = null,
                    Balance = 1200.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 03),
                    OperationCurrency = "UAH",
                    OperationAmount = 0,
                    Description = "Google Store",
                    Commission = 0.0,
                    MCC = "1000",
                },
            };

            var vm = new Page3VM(accounts, currencies, locations, categories, projects);

            vm.MonoAccount = accounts.FirstOrDefault();
            vm.SetMonoTransactions(transactions);

            vm.DeleteCommand.Execute(vm.FinancierTransactions.FirstOrDefault());

            Assert.Single(vm.FinancierTransactions);
        }
    }
}
