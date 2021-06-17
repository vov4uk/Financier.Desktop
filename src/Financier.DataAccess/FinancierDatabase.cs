using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.DataAccess.DataBase.Scripts;
using Financier.DataAccess.Monobank;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace Financier.DataAccess
{
    public class FinancierDatabase : IUnitOfWorkFactory
    {
        private readonly DbConnection _connection;

        public FinancierDatabase()
            : this(
                new DbContextOptionsBuilder<FinancierDataContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection($"Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();

        protected FinancierDatabase(DbContextOptions<FinancierDataContext> contextOptions)
        {
            ContextOptions = contextOptions;

            //Seed();
        }

        protected DbContextOptions<FinancierDataContext> ContextOptions { get; }

        private void Seed()
        {
            using var context = new FinancierDataContext(ContextOptions);
            //context.Database.EnsureDeleted();
            // context.Database.EnsureCreated();

            List<Task> createTasks = new List<Task>();

            ResourceSet create = SQL_create_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in create)
            {
                string resourceKey = entry.Key.ToString();
                object resource = entry.Value;
                createTasks.Add(context.Database.ExecuteSqlRawAsync(Convert.ToString(resource)));
            }
            Task.WaitAll(createTasks.ToArray());

            var alter = SQL_alter_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                .Cast<DictionaryEntry>()
                .Select(entry => new KeyValuePair<string, string>(Convert.ToString(entry.Key), Convert.ToString(entry.Value)))
                .OrderBy(x => x.Key)
                .ToList();
            foreach (var entry in alter)
            {
                context.Database.ExecuteSqlRaw(entry.Value);
            }

            var view = SQL_views_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                .Cast<DictionaryEntry>()
                .Select(entry => new KeyValuePair<string, string>(Convert.ToString(entry.Key), Convert.ToString(entry.Value)))
                .OrderBy(x => x.Key)
                .ToList();
            foreach (var entry in view)
            {
                context.Database.ExecuteSqlRaw(entry.Value);
            }

            context.SaveChanges();
        }

        public async Task Import(List<Entity> entities)
        {
            Seed();

            await using (var context = new FinancierDataContext(ContextOptions))
            {
                await context.AddRangeAsync(entities.OfType<IIdentity>().Where(x => x.Id > 0));

                foreach (var item in Backup.RESTORE_SCRIPTS)
                {
                    var sql = SQL_alter_files.ResourceManager.GetString(item);
                    await context.Database.ExecuteSqlRawAsync(sql);
                }

                await context.SaveChangesAsync();
            }

            var accounts = entities.OfType<Account>().ToList();
            foreach (var item in accounts)
            {
                await RebuildRunningBalanceForAccount(item.Id);
            }
        }

        public async Task RebuildRunningBalanceForAccount(int accountId)
        {
            await using (var context = new FinancierDataContext(ContextOptions))
            {
                await context.Database.ExecuteSqlRawAsync($"delete from running_balance where account_id={accountId}");
                await context.SaveChangesAsync();

                var transactions = await context.BlotterTransactionsForAccountWithSplits.Where(x => x.from_account_id == accountId).OrderBy(x => x.datetime).ThenBy(x => x._id).ToListAsync();
                long balance = 0;

                foreach (var transaction in transactions)
                {
                    var parentId = transaction.parent_id;
                    var isTransfer = transaction.is_transfer;
                    if (parentId > 0)
                    {
                        if (isTransfer >= 0)
                        {
                            // we only interested in the second part of the transfer-split
                            // which is marked with is_transfer=-1 (see v_blotter_for_account_with_splits)
                            continue;
                        }
                    }
                    var fromAccountId = transaction.from_account_id;
                    var toAccountId = transaction.to_account_id;
                    if (toAccountId > 0 && toAccountId == fromAccountId)
                    {
                        // weird bug when a transfer is done from an account to the same account
                        continue;
                    }
                    balance += transaction.from_amount;
                    await context.RunningBalance.AddAsync(new RunningBalance { Balance = (int)balance, AccountId = accountId, TransactionId = transaction._id });
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task ImportMonoTransactions(int accountId, List<MonoTransaction> transactions)
        {
            using var uow = CreateUnitOfWork();
            var currencies = await uow.GetRepository<Currency>().GetAllAsync();
            var locations = await uow.GetRepository<Location>().GetAllAsync();

            List<Transaction> transToAdd = new List<Transaction>();
            foreach (var x in transactions)
            {
                var locationId = locations.FirstOrDefault(l => l.Name.Contains(x.Description, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
                var newTr = new Transaction
                {
                    Id = 0,
                    FromAccountId = accountId,
                    FromAmount = Convert.ToInt64(x.CardCurrencyAmount * 100.0),
                    OriginalFromAmount = x.ExchangeRate == null ? 0 : Convert.ToInt64(x.OperationAmount * 100.0),
                    OriginalCurrencyId = x.ExchangeRate == null ? 0 : currencies.FirstOrDefault(c => c.Name == x.OperationCurrency)?.Id ?? 0,
                    CategoryId = 0,
                    LocationId = locationId,
                    Note = locationId > 0 ? null : x.Description,
                    DateTime = new DateTimeOffset(x.Date).ToUnixTimeMilliseconds()
                };
                transToAdd.Add(newTr);
            }

            await uow.GetRepository<Transaction>().AddRangeAsync(transToAdd);
            await uow.SaveChangesAsync();
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork<FinancierDataContext>(new FinancierDataContext(ContextOptions));
        }
    }
}