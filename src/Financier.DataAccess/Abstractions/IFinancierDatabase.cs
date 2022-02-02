using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Financier.DataAccess.Data;

namespace Financier.DataAccess.Abstractions
{
    public interface IFinancierDatabase : IUnitOfWorkFactory, IDisposable
    {
        Task ImportEntitiesAsync(IEnumerable<Entity> entities);

        Task RebuildAccountBalanceAsync(int accountId);

        Task AddTransactionsAsync(IEnumerable<Transaction> transactions);

        Task<T> GetOrCreateAsync<T>(int id)
            where T : class, IIdentity, new();

        Task<List<T>> ExecuteQuery<T>(string query)
            where T : class, new();

        Task<Transaction> GetOrCreateTransactionAsync(int id);

        Task<IEnumerable<Transaction>> GetSubTransactionsAsync(int id);

        Task InsertOrUpdateAsync<T>(IEnumerable<T> entities)
            where T : Entity, IIdentity;

        Task SaveAsFile(string dest);
    }
}
