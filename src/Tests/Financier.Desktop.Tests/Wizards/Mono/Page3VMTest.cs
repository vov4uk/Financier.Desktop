namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.Desktop.Wizards;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Newtonsoft.Json;
    using Xunit;

    public class Page3VMTest
    {
        [Theory]
        [AutoMoqData]
        public void MonoAccount_SetValue_AccountsNotContainsMonoAccount(
            List<AccountFilterModel> accounts)
        {
            var monoAccount = accounts.FirstOrDefault();
            DbManual.SetupTests(accounts);
            var vm = new Page3VM();

            vm.MonoAccount = monoAccount;

            accounts.Remove(monoAccount);
            Assert.True(vm.Accounts.SequenceEqual(accounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder)));
        }

        [Theory]
        [AutoMoqData]
        public void SetMonoTransactions_SetValue_TransformToFinancierTransactions(AccountFilterModel monoAccount)
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

            List<CategoryModel> categories = new List<CategoryModel> { new CategoryModel { Title = "Hot water", Id = 100 } };
            List<LocationModel> locations = new List<LocationModel> { new LocationModel { Id = 200, Title = "Google", Address = "Google Store" } };
            List<CurrencyModel> currencies = new List<CurrencyModel> { new CurrencyModel { Id = 1, Name = "USD" }, new CurrencyModel { Id = 2, Name = "UAH" } };
            DbManual.SetupTests(categories);
            DbManual.SetupTests(locations);
            DbManual.SetupTests(currencies);
            DbManual.SetupTests(new List<AccountFilterModel>() { monoAccount });
            var vm = new Page3VM();

            vm.MonoAccount = monoAccount;
            vm.SetMonoTransactions(transactions);

            Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(vm.FinancierTransactions));
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void DeleteCommand_Execute_TransactionsRemoved(
            AccountFilterModel account)
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

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<LocationModel>());
            DbManual.SetupTests(new List<CategoryModel>());
            var vm = new Page3VM();

            vm.MonoAccount = account;
            vm.SetMonoTransactions(transactions);

            vm.DeleteCommand.Execute(vm.FinancierTransactions.FirstOrDefault());

            Assert.Single(vm.FinancierTransactions);
            DbManual.ResetAllManuals();
        }
    }
}
