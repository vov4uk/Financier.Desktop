namespace Financier.DataAccess.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Financier.DataAccess;
    using Financier.DataAccess.Data;
    using Financier.DataAccess.View;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class FinancierDatabaseTest
    {
        [Fact]
        public void Constructor_NoParameters_AllRepositoriesCreated()
        {
            var db = new FinancierDatabase();

            using (var uow = db.CreateUnitOfWork())
            {
                Assert.NotNull(uow.GetRepository<Account>());
                Assert.NotNull(uow.GetRepository<AttributeDefinition>());
                Assert.NotNull(uow.GetRepository<CategoryAttribute>());
                Assert.NotNull(uow.GetRepository<TransactionAttribute>());
                Assert.NotNull(uow.GetRepository<Budget>());
                Assert.NotNull(uow.GetRepository<Category>());
                Assert.NotNull(uow.GetRepository<Currency>());
                Assert.NotNull(uow.GetRepository<Location>());
                Assert.NotNull(uow.GetRepository<Project>());
                Assert.NotNull(uow.GetRepository<Transaction>());
                Assert.NotNull(uow.GetRepository<Payee>());
                Assert.NotNull(uow.GetRepository<CCardClosingDate>());
                Assert.NotNull(uow.GetRepository<SmsTemplate>());
                Assert.NotNull(uow.GetRepository<CurrencyExchangeRate>());
                Assert.NotNull(uow.GetRepository<RunningBalance>());
                Assert.NotNull(uow.GetRepository<BlotterTransactionsForAccountWithSplits>());
            }
        }

        [Fact]
        public async Task ImportMonoTransactions_TtransacrionsInvalid_CatchException()
        {
            var db = new FinancierDatabase();
            await db.Seed();
            await this.PredefineData(db);
            List<Transaction> transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    Id = 0,
                    OriginalFromAmount = null,
                },
            };

            Task Result() => db.AddTransactionsAsync(transactions);

            await Assert.ThrowsAsync<DbUpdateException>(Result);
        }

        [Fact]
        public async Task ImportMonoTransactions_TtransacrionsValid_TransactionsAdded()
        {
            var db = new FinancierDatabase();
            await db.Seed();

            var initData = await this.PredefineData(db);

            List<Transaction> transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    Id = 0,
                    FromAmount = 100,
                    CategoryId = initData.category.Id,
                    OriginalCurrencyId = 0,
                    FromAccountId = initData.account.Id,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
                new Transaction()
                {
                    Id = 0,
                    FromAmount = 100,
                    CategoryId = initData.category.Id,
                    OriginalCurrencyId = 0,
                    FromAccountId = initData.account.Id,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
                new Transaction()
                {
                    Id = 0,
                    FromAmount = 100,
                    CategoryId = initData.category.Id,
                    OriginalCurrencyId = 0,
                    FromAccountId = initData.account.Id,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
            };

            await db.AddTransactionsAsync(transactions);

            using (var uow = db.CreateUnitOfWork())
            {
                var trRepo = uow.GetRepository<Transaction>();
                var allTr = await trRepo.GetAllAsync();
                Assert.Equal(transactions.Count, allTr.Count);
            }
        }

        [Fact]
        public async Task RebuildRunningBalanceForAccount_Have3Transactions_BalanceUpdated()
        {
            var db = new FinancierDatabase();
            await db.Seed();

            var initData = await this.PredefineData(db);

            List<Transaction> transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    Id = 0,
                    FromAmount = -100,
                    CategoryId = initData.category.Id,
                    OriginalCurrencyId = 0,
                    FromAccountId = initData.account.Id,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
                new Transaction()
                {
                    Id = 0,
                    FromAmount = -200,
                    CategoryId = initData.category.Id,
                    OriginalCurrencyId = 0,
                    FromAccountId = initData.account.Id,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
                new Transaction()
                {
                    Id = 0,
                    FromAmount = -300,
                    CategoryId = initData.category.Id,
                    OriginalCurrencyId = 0,
                    FromAccountId = initData.account.Id,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
            };

            await db.AddTransactionsAsync(transactions);

            await db.RebuildRunningBalanceForAccount(initData.account.Id);

            using (var uow = db.CreateUnitOfWork())
            {
                var balanceRepo = uow.GetRepository<RunningBalance>();
                var allTr = await balanceRepo.GetAllAsync();
                Assert.Equal(transactions.Count, allTr.Count);
                Assert.Equal(-600, allTr[2].Balance);
            }
        }

        [Fact]
        public async Task Import_DieferentEntityTypes_AllAddedToDB()
        {
            var db = new FinancierDatabase();

            List<Entity> entities = new List<Entity>()
            {
                new Currency() { Id = 1, Title = "Dollar", IsDefault = true, IsActive = true, Name = "USD", Decimals = 2, Symbol = "$", SymbolFormat = "." },
                new Category() { Id = 1, IsActive = true, Title = "Default category", LastLocationId = 0, LastProjectId = 0, Type = "Expanse" },
                new Account() { Id = 1, Title = "Default account", CurrencyId = 1, Type = "CASH", IsActive = true },
                new Transaction()
                {
                    Id = 1,
                    FromAmount = -100,
                    CategoryId = 1,
                    OriginalCurrencyId = 0,
                    FromAccountId = 1,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
                new Transaction()
                {
                    Id = 2,
                    FromAmount = -200,
                    CategoryId = 1,
                    OriginalCurrencyId = 0,
                    FromAccountId = 1,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
                new Transaction()
                {
                    Id = 3,
                    FromAmount = -300,
                    CategoryId = 1,
                    OriginalCurrencyId = 0,
                    FromAccountId = 1,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
            };

            await db.ImportEntities(entities);

            using (var uow = db.CreateUnitOfWork())
            {
                var balanceRepo = uow.GetRepository<RunningBalance>();
                var balanceTr = await balanceRepo.GetAllAsync();
                Assert.Equal(3, balanceTr.Count);
                Assert.Equal(-600, balanceTr[2].Balance);

                var trRepo = uow.GetRepository<Transaction>();
                var allTr = await trRepo.GetAllAsync();
                Assert.Equal(3, allTr.Count);

                Assert.Single(await uow.GetRepository<Account>().GetAllAsync());
                Assert.Single(await uow.GetRepository<Currency>().GetAllAsync());
                Assert.Equal(3, (await uow.GetRepository<Category>().GetAllAsync()).Count);
            }
        }

        private async Task<(Account account, Currency currency, Category category)> PredefineData(FinancierDatabase db)
        {
            using (var uow = db.CreateUnitOfWork())
            {
                var currency = new Currency() { Id = 0, Title = "Dollar", IsDefault = true, IsActive = true, Name = "USD", Decimals = 2, Symbol = "$", SymbolFormat = "." };
                await uow.GetRepository<Currency>().AddAsync(currency);
                var category = new Category() { Id = 0, IsActive = true, Title = "Default category", LastLocationId = 0, LastProjectId = 0, Type = "Expanse" };
                var account = new Account() { Id = 0, Title = "Default account", Currency = currency, Type = "CASH", IsActive = true };
                await uow.GetRepository<Account>().AddAsync(account);

                await uow.GetRepository<Category>().AddAsync(category);
                await uow.SaveChangesAsync();

                return (account, currency, category);
            }
        }
    }
}
