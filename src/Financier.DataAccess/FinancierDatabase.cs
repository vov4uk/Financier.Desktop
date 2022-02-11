using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.DataAccess.DataBase.Scripts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace Financier.DataAccess
{
    [ExcludeFromCodeCoverage]
    public class FinancierDatabase : IFinancierDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly DbConnection _connection;
        private bool isDisposed;

        internal FinancierDatabase()
            : this(
                new DbContextOptionsBuilder<FinancierDataContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        protected FinancierDatabase(DbContextOptions<FinancierDataContext> contextOptions)
        {
            ContextOptions = contextOptions;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected DbContextOptions<FinancierDataContext> ContextOptions { get; }

        internal async Task Seed()
        {
            using var context = new FinancierDataContext(ContextOptions);

            ResourceSet create = SQL_create_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in create)
            {
                await context.Database.ExecuteSqlRawAsync(Convert.ToString(entry.Value));
            }

            var alter = SQL_alter_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                .Cast<DictionaryEntry>()
                .Select(entry => new KeyValuePair<string, string>(Convert.ToString(entry.Key), Convert.ToString(entry.Value)))
                .OrderBy(x => x.Key)
                .ToList();
            foreach (var entry in alter)
            {
                await context.Database.ExecuteSqlRawAsync(entry.Value);
            }

            var view = SQL_views_files.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true)
                .Cast<DictionaryEntry>()
                .Select(entry => new KeyValuePair<string, string>(Convert.ToString(entry.Key), Convert.ToString(entry.Value)))
                .OrderBy(x => x.Key)
                .ToList();
            foreach (var entry in view)
            {
                await context.Database.ExecuteSqlRawAsync(entry.Value);
            }

            await context.SaveChangesAsync();
        }

        public async Task ImportEntitiesAsync(IEnumerable<Entity> entities)
        {
            await Seed();

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
                await RebuildAccountBalanceAsync(item.Id);
            }
        }

        public async Task RebuildAccountBalanceAsync(int accountId)
        {
            await using (var context = new FinancierDataContext(ContextOptions))
            {
                await context.Database.ExecuteSqlRawAsync("delete from running_balance where account_id=@p0", accountId);
                await context.SaveChangesAsync();

                var transactions = await context.BlotterTransactions.Where(x => x.FromAccountId == accountId).OrderBy(x => x.DateTime).ToListAsync();
                long balance = 0;

                foreach (var transaction in transactions)
                {
                    if (transaction.ParentId > 0 && transaction.IsTransfer >= 0)
                    {
                        // we only interested in the second part of the transfer-split
                        // which is marked with is_transfer=-1 (see v_blotter_for_account_with_splits)
                        continue;
                    }
                    var toAccountId = transaction.ToAccountId;
                    if (toAccountId > 0 && toAccountId == transaction.FromAccountId)
                    {
                        // weird bug when a transfer is done from an account to the same account
                        continue;
                    }
                    balance += transaction.FromAmount;
                    await context.RunningBalance.AddAsync(new RunningBalance { Balance = balance, AccountId = accountId, TransactionId = transaction.Id });
                }

                var acc = context.Accounts.FirstOrDefault(x => x.Id == accountId);
                acc.TotalAmount = balance;
                var lastTransaction = transactions.LastOrDefault();
                acc.LastTransactionDate = lastTransaction?.DateTime ?? 0;
                context.Accounts.Update(acc);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddTransactionsAsync(IEnumerable<Transaction> transactions)
        {
            using var uow = CreateUnitOfWork();
            await uow.GetRepository<Transaction>().AddRangeAsync(transactions);

            await uow.SaveChangesAsync();
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork<FinancierDataContext>(new FinancierDataContext(ContextOptions));
        }

        public async Task<T> GetOrCreateAsync<T>(int id)
            where T : class, IIdentity, new()
        {
            if (id != 0)
            {
                using var uow = CreateUnitOfWork();
                return await uow.GetRepository<T>().FindByAsync(x => x.Id == id);
            }
            return new T { Id = 0 };
        }

        public async Task<Transaction> GetOrCreateTransactionAsync(int id)
        {
            if (id != 0)
            {
                using var uow = CreateUnitOfWork();
                return await uow.GetRepository<Transaction>().FindByAsync(x => x.Id == id, x => x.FromAccount);
            }

            return new Transaction { DateTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(), Id = 0, CategoryId = 0 };
        }

        public async Task<IEnumerable<Transaction>> GetSubTransactionsAsync(int id)
        {
            if (id != 0)
            {
                using var uow = CreateUnitOfWork();
                return (await uow.GetRepository<Transaction>().FindManyAsync(
                    predicate: x => x.ParentId == id,
                    includes: new Expression<Func<Transaction, object>>[]{ o => o.OriginalCurrency, c => c.Category})) ?? Array.Empty<Transaction>().ToList();
            }

            return Array.Empty<Transaction>();
        }

        public async Task InsertOrUpdateAsync<T>(IEnumerable<T> entities)
            where T : Entity, IIdentity
        {
            using var uow = CreateUnitOfWork();
            var trRepo = uow.GetRepository<T>();
            foreach (var item in entities)
            {
                item.UpdatedOn = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                if (item.Id == 0)
                {
                    await trRepo.AddAsync(item);
                }
                else
                {
                    await trRepo.UpdateAsync(item);
                }
            }
            await uow.SaveChangesAsync();
        }

        public async Task<List<T>> ExecuteQuery<T>(string query) where T : class, new()
        {
            await using (var db = new FinancierDataContext(ContextOptions))
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {
                Logger.Info(query);
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                await db.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    var lst = new List<T>();
                    while (await reader.ReadAsync())
                    {
                        var newObject = new T();
                        foreach (PropertyInfo property in newObject.GetType().GetProperties())
                        {
                            ColumnAttribute customAttribute = Attribute.GetCustomAttribute(property, typeof(ColumnAttribute)) as ColumnAttribute;
                            if (customAttribute != null)
                            {
                                int ordinal = reader.GetOrdinal(customAttribute.Name);
                                object obj = ordinal != -1 ?
                                    reader.GetValue(ordinal) :
                                    throw new Exception(string.Format("Class [{0}] have attribute of field [{1}] which not exist in reader", this.GetType(), customAttribute.Name));

                                if (obj != DBNull.Value)
                                {
                                    property.SetValue(newObject, Unbox(obj, property.PropertyType), null);
                                }
                            }
                        }
                        lst.Add(newObject);
                    }

                    return lst;
                }
            }
        }

        public async Task SaveAsFile(string dest)
        {
            await using (var db = new FinancierDataContext(ContextOptions))
            using (var command = db.Database.GetDbConnection().CreateCommand())
            {
                string query = $"VACUUM main INTO '{dest}'";
                Logger.Info(query);
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                await db.Database.OpenConnectionAsync();

                await command.ExecuteNonQueryAsync();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _connection.Dispose();
            }

            this.isDisposed = true;
        }

        static object Unbox(object x, Type t)
        {
            var underlyingType = Nullable.GetUnderlyingType(t);
            if (Nullable.GetUnderlyingType(t) != null)
            {
                return Convert.ChangeType(x, underlyingType);
            }
            return Convert.ChangeType(x, t);
        }
    }
}