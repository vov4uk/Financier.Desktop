namespace Financier.DataAccess.Tests
{
    using System.Collections.Generic;
    using System.Linq;
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
                Assert.NotNull(uow.GetRepository<BlotterTransactions>());
            }
        }

        [Fact]
        public async Task AddTransactionsAsync_TtransacrionsInvalid_CatchException()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            await PredefineData(db);
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
        public async Task AddTransactionsAsync_TtransacrionsValid_TransactionsAdded()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            var initData = await PredefineData(db);

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
            await db.SeedAsync();

            var initData = await PredefineData(db);

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

            await db.RebuildAccountBalanceAsync(initData.account.Id);

            using (var uow = db.CreateUnitOfWork())
            {
                var balanceRepo = uow.GetRepository<RunningBalance>();
                var allTr = await balanceRepo.GetAllAsync();
                Assert.Equal(transactions.Count, allTr.Count);
                Assert.Equal(-600, allTr[2].Balance);

                var acc = await uow.GetRepository<Account>().FindByAsync(x => x.Id == initData.account.Id);
                Assert.Equal(-600, acc.TotalAmount);
            }
        }

        [Fact]
        public async Task ImportEntities_DieferentEntityTypes_AllAddedToDB()
        {
            var db = new FinancierDatabase();

            List<Entity> entities = new List<Entity>()
            {
                new Currency() { Id = 1, Title = "Dollar", IsDefault = true, IsActive = true, Name = "USD", Decimals = 2, Symbol = "$", SymbolFormat = "." },
                new Category() { Id = 1, IsActive = true, Title = "Default category", LastLocationId = 0, LastProjectId = 0, Type = 0 },
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

            await db.ImportEntitiesAsync(entities);

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

        [Fact]
        public async Task GetOrCreate_NewItem_CreateItem()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            var account = await db.GetOrCreateAsync<Account>(0);

            Assert.Equal(0, account.Id);
        }

        [Fact]
        public async Task GetOrCreate_ItemNotExist_ReturnNull()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            var account = await db.GetOrCreateAsync<Account>(1);

            Assert.Null(account);
        }

        [Fact]
        public async Task GetOrCreate_ItemExist_GetItem()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            await PredefineData(db);

            var account = await db.GetOrCreateAsync<Account>(1);
            var currency = await db.GetOrCreateAsync<Currency>(1);
            var category = await db.GetOrCreateAsync<Category>(1);

            Assert.Equal(1, account.Id);
            Assert.Equal(1, currency.Id);
            Assert.Equal(1, category.Id);
        }

        [Fact]
        public async Task GetSubTransactions_IdIs0_EmptyCollection()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            var transactions = await db.GetSubTransactionsAsync(0);

            Assert.Empty(transactions);
        }

        [Fact]
        public async Task GetSubTransactions_ItemNotExist_EmptyCollection()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            var transactions = await db.GetSubTransactionsAsync(1);

            Assert.Empty(transactions);
        }

        [Fact]
        public async Task GetSubTransactions_ItemExist_GetItem()
        {
            var db = new FinancierDatabase();
            List<Entity> entities = new List<Entity>()
            {
                new Currency() { Id = 1, Title = "Dollar", IsDefault = true, IsActive = true, Name = "USD", Decimals = 2, Symbol = "$", SymbolFormat = "." },
                new Category() { Id = 1, IsActive = true, Title = "Default category", LastLocationId = 0, LastProjectId = 0, Type = 0 },
                new Account() { Id = 1, Title = "Default account", CurrencyId = 1, Type = "CASH", IsActive = true },
                new Transaction()
                {
                    Id = 1,
                    FromAmount = -100,
                    CategoryId = -1,
                    OriginalCurrencyId = 1,
                    FromAccountId = 1,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
                new Transaction()
                {
                    Id = 2,
                    ParentId = 1,
                    FromAmount = -50,
                    CategoryId = 0,
                    OriginalCurrencyId = 1,
                    FromAccountId = 1,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
                new Transaction()
                {
                    Id = 3,
                    ParentId = 1,
                    FromAmount = -50,
                    CategoryId = 1,
                    OriginalCurrencyId = 1,
                    FromAccountId = 1,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
            };
            await db.ImportEntitiesAsync(entities);

            var transactions = (await db.GetSubTransactionsAsync(1)).ToArray();

            Assert.Equal(2, transactions.Length);
            Assert.NotNull(transactions[0].Category);
            Assert.NotNull(transactions[0].OriginalCurrency);
            Assert.Equal(2, transactions[0].Id);
        }

        [Fact]
        public async Task GetOrCreateTransaction_IdIs0_CreateItem()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            var transaction = await db.GetOrCreateTransactionAsync(0);

            Assert.Equal(0, transaction.Id);
            Assert.Equal(0, transaction.CategoryId);
            Assert.True(transaction.DateTime > 0);
        }

        [Fact]
        public async Task GetOrCreateTransaction_ItemNotExist_ReturnNull()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            var transaction = await db.GetOrCreateTransactionAsync(1);

            Assert.Null(transaction);
        }

        [Fact]
        public async Task GetOrCreateTransaction_ItemExist_GetItem()
        {
            var db = new FinancierDatabase();
            List<Entity> entities = new List<Entity>()
            {
                new Currency() { Id = 1, Title = "Dollar", IsDefault = true, IsActive = true, Name = "USD", Decimals = 2, Symbol = "$", SymbolFormat = "." },
                new Category() { Id = 1, IsActive = true, Title = "Default category", LastLocationId = 0, LastProjectId = 0, Type = 0 },
                new Account() { Id = 1, Title = "Default account", CurrencyId = 1, Type = "CASH", IsActive = true },
                new Transaction()
                {
                    Id = 1,
                    FromAmount = -100,
                    CategoryId = 1,
                    OriginalCurrencyId = 1,
                    FromAccountId = 1,
                    OriginalFromAmount = 0,
                    DateTime = 1639121044000,
                },
            };
            await db.ImportEntitiesAsync(entities);

            var transaction = await db.GetOrCreateTransactionAsync(1);

            Assert.NotNull(transaction.FromAccount);
            Assert.Equal(1, transaction.Id);
        }

        [Fact]
        public async Task InsertOrUpdate_IdIs0_CreateItem()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            await db.InsertOrUpdateAsync(new[] { new Currency() { Id = 0, Title = "Dollar", IsDefault = true, IsActive = true, Name = "USD", Decimals = 2, Symbol = "$", SymbolFormat = "." } });

            using (var uow = db.CreateUnitOfWork())
            {
                var trRepo = uow.GetRepository<Currency>();
                var dollar = await trRepo.FindByAsync(x => x.Title == "Dollar");
                Assert.Equal("USD", dollar.Name);
            }
        }

        [Fact]
        public async Task InsertOrUpdate_ItemExist_UpdateItem()
        {
            var db = new FinancierDatabase();
            await db.SeedAsync();
            var initData = await PredefineData(db);

            initData.currency.Name = "UAH";
            await db.InsertOrUpdateAsync(new[] { initData.currency });

            using (var uow = db.CreateUnitOfWork())
            {
                var trRepo = uow.GetRepository<Currency>();
                var dollar = await trRepo.FindByAsync(x => x.Title == "Dollar");
                Assert.Equal("UAH", dollar.Name);
            }
        }

        [Fact]
        public void CreateUnitOfWork_Execute_UOWCreated()
        {
            var db = new FinancierDatabase();
            var uow = db.CreateUnitOfWork();

            Assert.True(uow is UnitOfWork<FinancierDataContext>);
        }

        private static async Task<(Account account, Currency currency, Category category)> PredefineData(FinancierDatabase db)
        {
            using (var uow = db.CreateUnitOfWork())
            {
                var currency = new Currency() { Id = 0, Title = "Dollar", IsDefault = true, IsActive = true, Name = "USD", Decimals = 2, Symbol = "$", SymbolFormat = "." };
                await uow.GetRepository<Currency>().AddAsync(currency);
                var category = new Category() { Id = 0, IsActive = true, Title = "Default category", LastLocationId = 0, LastProjectId = 0, Type = 0 };
                var account = new Account() { Id = 0, Title = "Default account", Currency = currency, Type = "CASH", IsActive = true };
                await uow.GetRepository<Account>().AddAsync(account);

                await uow.GetRepository<Category>().AddAsync(category);
                await uow.SaveChangesAsync();

                return (account, currency, category);
            }
        }
    }
}
