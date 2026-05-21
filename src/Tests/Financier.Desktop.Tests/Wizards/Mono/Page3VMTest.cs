namespace Financier.Desktop.Tests.Wizards.Mono
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Financier.Common.Entities;
    using Financier.Common.Model;
    using Financier.Desktop.Data;
    using Financier.Desktop.Helpers;
    using Financier.Desktop.Pages.Dialogs;
    using Financier.Desktop.Wizards;
    using Financier.Desktop.Wizards.MonoWizard.ViewModel;
    using Financier.Tests.Common;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    public class Page3VMTest
    {
        private readonly Mock<IDialogWrapper> _dialogWrapperMock = new Mock<IDialogWrapper>();

        [Theory]
        [AutoMoqData]
        public void MonoAccount_SetValue_AccountsNotContainsMonoAccount(
            List<AccountFilterModel> accounts)
        {
            var monoAccount = accounts.FirstOrDefault();
            DbManual.SetupTests(accounts);
            var vm = new Page3VM(_dialogWrapperMock.Object);

            vm.MonoAccount = monoAccount;

            accounts.Remove(monoAccount);
            Assert.True(vm.Accounts.SequenceEqual(accounts.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder)));
        }

        [Theory]
        [AutoMoqData]
        public async Task SetMonoTransactions_SetValue_TransformToFinancierTransactions(AccountFilterModel monoAccount)
        {
            var rules = new List<RuleModel>
            {
                new RuleModel
                {
                 Id = 8,
                 Created = new DateTime(2026, 5, 16, 22, 32, 55, 354),
                 Condition = "Description contains",
                 Description = "Google",
                 Title = null,
                 IsActive = true,
                 PayeeId = null,
                 ProjectId = null,
                 CategoryId = null,
                 LocationId = 200,
                 MCCCategory = null,
                },
                new RuleModel
                {
                 Id = 0,
                 Created = new DateTime(2026, 5, 16, 22, 32, 55, 354),
                 Condition = "Description contains",
                 Description = "Hot Water",
                 Title = null,
                 IsActive = true,
                 PayeeId = null,
                 ProjectId = null,
                 CategoryId = 100,
                 LocationId = null,
                 MCCCategory = null,
                },
            };

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
                    Note = "Google",
                    MCC = 1000,
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
                    Note = "Google Store",
                    MCC = 1000,
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
                    Note = "Hot Water",
                    MCC = 1000,
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
                    MCC = 1000,
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
                    MCC = 1000,
                    DateTime = new DateTimeOffset(new DateTime(2021, 12, 04, 21, 12, 00)).ToUnixTimeMilliseconds(),
                },
            };

            List<CategoryModel> categories = new List<CategoryModel> { new CategoryModel { Title = "Hot water", Id = 100 } };
            List<LocationModel> locations = new List<LocationModel> { new LocationModel { Id = 200, Title = "Google", Address = "Google Store", IsActive = true } };
            List<CurrencyModel> currencies = new List<CurrencyModel> { new CurrencyModel { Id = 1, Name = "USD" }, new CurrencyModel { Id = 2, Name = "UAH" } };
            DbManual.SetupTests(categories);
            DbManual.SetupTests(locations);
            DbManual.SetupTests(currencies);
            DbManual.SetupTests(new List<AccountFilterModel>() { monoAccount });
            DbManual.SetupTests(rules);

            var vm = new Page3VM(_dialogWrapperMock.Object);

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
            List<BankTransaction> transactions = GetBankTransactions();

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<LocationModel>());
            DbManual.SetupTests(new List<CategoryModel>());
            var vm = new Page3VM(_dialogWrapperMock.Object);

            vm.MonoAccount = account;
            vm.SetMonoTransactions(transactions);

            vm.DeleteCommand.Execute(vm.FinancierTransactions.FirstOrDefault());

            Assert.Single(vm.FinancierTransactions);
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void ClearAllNotes_Execute_NotesEmpty(
            AccountFilterModel account)
        {
            List<BankTransaction> transactions = GetBankTransactions();

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<LocationModel>());
            DbManual.SetupTests(new List<CategoryModel>());
            var vm = new Page3VM(_dialogWrapperMock.Object);

            vm.MonoAccount = account;
            vm.SetMonoTransactions(transactions);

            vm.ClearAllNotesCommand.Execute();

            foreach (var item in vm.FinancierTransactions)
            {
                Assert.Null(item.Note);
            }
        }

        [Theory]
        [AutoMoqData]
        public void ApplyRules_DescriptionContainsCondition_UpdatesTransactionCategory(
            AccountFilterModel account)
        {
            // Arrange
            var categoryId = 100;
            var rule = new RuleModel
            {
                Id = 101,
                IsActive = true,
                Condition = "Description contains",
                Description = "Amazon",
                CategoryId = categoryId,
                Created = DateTime.Now,
            };

            var transaction = new BankTransaction
            {
                Description = "Payment to Amazon Store",
                CardCurrencyAmount = 100.0,
                ExchangeRate = null,
                Balance = 1200.0,
                Cashback = 3.0,
                Date = new DateTime(2021, 12, 04, 21, 12, 03),
                OperationCurrency = "UAH",
                OperationAmount = 0,
                Commission = 0.0,
                MCC = "1000",
            };

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<RuleModel> { rule });

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(new List<BankTransaction> { transaction });

            // Assert
            Assert.Single(vm.FinancierTransactions);
            Assert.Equal(categoryId, vm.FinancierTransactions[0].CategoryId);
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void ApplyRules_DescriptionMatchesCondition_UpdatesTransactionCategory(
            AccountFilterModel account)
        {
            // Arrange
            var categoryId = 100;
            var payeeId = 50;
            var rule = new RuleModel
            {
                Id = 100,
                IsActive = true,
                Condition = "Description matches",
                Description = "Exact Match",
                CategoryId = categoryId,
                PayeeId = payeeId,
                Created = DateTime.Now,
            };

            var transaction = new BankTransaction
            {
                Description = "Exact Match",
                CardCurrencyAmount = 100.0,
                ExchangeRate = null,
                Balance = 1200.0,
                Cashback = 3.0,
                Date = new DateTime(2021, 12, 04, 21, 12, 03),
                OperationCurrency = "UAH",
                OperationAmount = 0,
                Commission = 0.0,
                MCC = "1000",
            };

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<RuleModel> { rule });

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(new List<BankTransaction> { transaction });

            // Assert
            Assert.Single(vm.FinancierTransactions);
            Assert.Equal(categoryId, vm.FinancierTransactions[0].CategoryId);
            Assert.Equal(payeeId, vm.FinancierTransactions[0].PayeeId);
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void ApplyRules_MCCCondition_UpdatesTransactionProperties(
            AccountFilterModel account)
        {
            // Arrange
            var categoryId = 100;
            var locationId = 75;
            var projectId = 25;
            var rule = new RuleModel
            {
                Id = 105,
                IsActive = true,
                Condition = "MCC",
                Description = "Готелі",
                CategoryId = categoryId,
                LocationId = locationId,
                ProjectId = projectId,
                Created = DateTime.Now,
            };

            var transaction = new BankTransaction
            {
                Description = "Hotel booking",
                CardCurrencyAmount = 100.0,
                ExchangeRate = null,
                Balance = 1200.0,
                Cashback = 3.0,
                Date = new DateTime(2021, 12, 04, 21, 12, 03),
                OperationCurrency = "UAH",
                OperationAmount = 0,
                Commission = 0.0,
                MCC = "3700",
            };

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<RuleModel> { rule });

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(new List<BankTransaction> { transaction });

            // Assert
            Assert.Single(vm.FinancierTransactions);
            Assert.Equal(categoryId, vm.FinancierTransactions[0].CategoryId);
            Assert.Equal(locationId, vm.FinancierTransactions[0].LocationId);
            Assert.Equal(projectId, vm.FinancierTransactions[0].ProjectId);
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void ApplyRules_NoMatchingRule_TransactionUnchanged(
            AccountFilterModel account)
        {
            // Arrange
            var rule = new RuleModel
            {
                Id = 102,
                IsActive = true,
                Condition = "Description contains",
                Description = "NonExistentText",
                CategoryId = 100,
                Created = DateTime.Now,
            };

            var transaction = new BankTransaction
            {
                Description = "Payment to Google Store",
                CardCurrencyAmount = 100.0,
                ExchangeRate = null,
                Balance = 1200.0,
                Cashback = 3.0,
                Date = new DateTime(2021, 12, 04, 21, 12, 03),
                OperationCurrency = "UAH",
                OperationAmount = 0,
                Commission = 0.0,
                MCC = "1000",
            };

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<RuleModel> { rule });

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(new List<BankTransaction> { transaction });

            // Assert
            Assert.Single(vm.FinancierTransactions);
            Assert.Equal(0, vm.FinancierTransactions[0].CategoryId);
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void ApplyRules_InactiveRule_TransactionUnchanged(
            AccountFilterModel account)
        {
            // Arrange
            var rule = new RuleModel
            {
                Id = 106,
                IsActive = false,
                Condition = "Description contains",
                Description = "Apple",
                CategoryId = 100,
                Created = DateTime.Now,
            };

            var transaction = new BankTransaction
            {
                Description = "Payment to Apple Store",
                CardCurrencyAmount = 100.0,
                ExchangeRate = null,
                Balance = 1200.0,
                Cashback = 3.0,
                Date = new DateTime(2021, 12, 04, 21, 12, 03),
                OperationCurrency = "UAH",
                OperationAmount = 0,
                Commission = 0.0,
                MCC = "1000",
            };

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<RuleModel> { rule });

            // Act
            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(new List<BankTransaction> { transaction });

            // Assert
            Assert.Single(vm.FinancierTransactions);
            Assert.Equal(0, vm.FinancierTransactions[0].CategoryId);
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void ApplyRules_CaseInsensitiveMatching_SuccessfullyMatches(
            AccountFilterModel account)
        {
            // Arrange
            var categoryId = 100;
            var rule = new RuleModel
            {
                Id = 103,
                IsActive = true,
                Condition = "Description contains",
                Description = "google",
                CategoryId = categoryId,
                Created = DateTime.Now,
            };

            var transaction = new BankTransaction
            {
                Description = "Payment to GOOGLE STORE",
                CardCurrencyAmount = 100.0,
                ExchangeRate = null,
                Balance = 1200.0,
                Cashback = 3.0,
                Date = new DateTime(2021, 12, 04, 21, 12, 03),
                OperationCurrency = "UAH",
                OperationAmount = 0,
                Commission = 0.0,
                MCC = "1000",
            };

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<RuleModel> { rule });

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(new List<BankTransaction> { transaction });

            // Assert
            Assert.Single(vm.FinancierTransactions);
            Assert.Equal(categoryId, vm.FinancierTransactions[0].CategoryId);
            DbManual.ResetAllManuals();
        }

        [Theory]
        [AutoMoqData]
        public void ApplyRules_MultipleRules_LastMatchingRuleApplied(
            AccountFilterModel account)
        {
            // Arrange
            var categoryId1 = 100;
            var categoryId2 = 200;
            var rule1 = new RuleModel
            {
                Id = 201,
                IsActive = true,
                Condition = "Description contains",
                Description = "Meta",
                CategoryId = categoryId1,
                Created = DateTime.Now,
            };

            var rule2 = new RuleModel
            {
                Id = 202,
                IsActive = true,
                Condition = "Description contains",
                Description = "Paymnt",
                CategoryId = categoryId2,
                Created = DateTime.Now.AddSeconds(1),
            };

            var transaction = new BankTransaction
            {
                Description = "Paymnt to Meta Store",
                CardCurrencyAmount = 100.0,
                ExchangeRate = null,
                Balance = 1200.0,
                Cashback = 3.0,
                Date = new DateTime(2021, 12, 04, 21, 12, 03),
                OperationCurrency = "UAH",
                OperationAmount = 0,
                Commission = 0.0,
                MCC = "1000",
            };

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<RuleModel> { rule1, rule2 });

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(new List<BankTransaction> { transaction });

            // Assert
            Assert.Single(vm.FinancierTransactions);
            Assert.Equal(categoryId2, vm.FinancierTransactions[0].CategoryId);
            DbManual.ResetAllManuals();
        }

        [Fact]
        public async Task OpenRulesDialogAsync_ValidRuleCreated_AddsRuleAndReappliesRules()
        {
            // Arrange
            var account = new AccountFilterModel { Id = 1 };
            var description = "Payment to Store";

            object ruleDto = new RuleDTO
            {
                Description = description,
                Condition = "Description contains",
                Created = DateTime.Now,
                IsActive = true,
                CategoryId = 1000,
            };

            _dialogWrapperMock
                .Setup(d => d.ShowDialog<RuleControl>(It.IsAny<RuleControlVM>(), 380, 400, "Rule"))
                .Returns(ruleDto);

            List<BankTransaction> transactions = new List<BankTransaction>
            {
                new BankTransaction
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = null,
                    Balance = 1100.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 04),
                    OperationCurrency = "UAH",
                    OperationAmount = 0,
                    Description = description,
                    Commission = 0.0,
                    MCC = "0",
                },
            };

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<LocationModel>());
            DbManual.SetupTests(new List<CategoryModel>());

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(transactions);

            // Act
            await vm.AddRuleCommand.Execute(new FinancierTransactionDto
            {
                Note = description,
                MonoAccountId = account.Id,
                FromAmount = 10000,
                MCC = 0,
            });

            // Assert
            Assert.NotEmpty(DbManual.Rules);
            var addedRule = DbManual.Rules.LastOrDefault();
            Assert.NotNull(addedRule);
            Assert.Equal(description, addedRule.Description);
            Assert.Equal("Description contains", addedRule.Condition);
            Assert.True(addedRule.IsActive);
            Assert.Equal(1000, addedRule.CategoryId);

            // Verify the transaction was updated with the new rule
            var transaction = vm.FinancierTransactions.FirstOrDefault();
            Assert.NotNull(transaction);
            Assert.Equal(1000, transaction.CategoryId);

            DbManual.ResetAllManuals();
        }

        [Fact]
        public async Task OpenRulesDialogAsync_DialogCancelled_RuleNotAdded()
        {
            // Arrange
            var account = new AccountFilterModel { Id = 1 };
            var description = "Payment to Store";

            _dialogWrapperMock
                .Setup(d => d.ShowDialog<RuleControl>(It.IsAny<RuleControlVM>(), 380, 400, "Rule"))
                .Returns((RuleDTO)null);

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<LocationModel>());
            DbManual.SetupTests(new List<CategoryModel>());
            DbManual.SetupTests(new List<RuleModel>());

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;

            await vm.AddRuleCommand.Execute(new FinancierTransactionDto
            {
                Note = description,
                MonoAccountId = account.Id,
                FromAmount = 10000,
                MCC = 0,
            });

            // Assert
            Assert.Empty(DbManual.Rules);
            DbManual.ResetAllManuals();
        }

        [Fact]
        public async Task OpenRulesDialogAsync_RuleCreatedWithAllProperties_AllPropertiesPreserved()
        {
            var account = new AccountFilterModel { Id = 1 };
            var description = "Restaurant Payment";

            var ruleDto = new RuleDTO
            {
                Description = description,
                Condition = "Description matches",
                Created = DateTime.Now,
                IsActive = true,
                CategoryId = 100,
                LocationId = 50,
                PayeeId = 25,
                ProjectId = 10,
            };

            List<BankTransaction> transactions = new List<BankTransaction>
            {
                new BankTransaction
                {
                    CardCurrencyAmount = 100.0,
                    ExchangeRate = null,
                    Balance = 1100.0,
                    Cashback = 3.0,
                    Date = new DateTime(2021, 12, 04, 21, 12, 04),
                    OperationCurrency = "UAH",
                    OperationAmount = 0,
                    Description = description,
                    Commission = 0.0,
                    MCC = "0",
                },
            };

            _dialogWrapperMock
                .Setup(d => d.ShowDialog<RuleControl>(It.IsAny<RuleControlVM>(), 380, 400, "Rule"))
                .Returns(ruleDto);

            DbManual.SetupTests(new List<AccountFilterModel>() { account });
            DbManual.SetupTests(new List<LocationModel>());
            DbManual.SetupTests(new List<CategoryModel>());
            DbManual.SetupTests(new List<RuleModel>());

            var vm = new Page3VM(_dialogWrapperMock.Object);
            vm.MonoAccount = account;
            vm.SetMonoTransactions(transactions);

            // Act
            await vm.AddRuleCommand.Execute(new FinancierTransactionDto
            {
                Note = description,
                MonoAccountId = account.Id,
                FromAmount = 10000,
                MCC = 0,
            });

            // Assert
            Assert.Single(DbManual.Rules);
            var addedRule = DbManual.Rules.First();
            Assert.Equal(description, addedRule.Description);
            Assert.Equal("Description matches", addedRule.Condition);
            Assert.True(addedRule.IsActive);
            Assert.Equal(100, addedRule.CategoryId);
            Assert.Equal(50, addedRule.LocationId);
            Assert.Equal(25, addedRule.PayeeId);
            Assert.Equal(10, addedRule.ProjectId);
        }

        private static List<BankTransaction> GetBankTransactions()
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
            return transactions;
        }
    }
}
