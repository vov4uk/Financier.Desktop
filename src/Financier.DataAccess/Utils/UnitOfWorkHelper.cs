using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Financier.DataAccess.Utils
{
    [ExcludeFromCodeCoverage]
    public static class UnitOfWorkHelper
    {
        public static async Task<List<T>> GetAllAsync<T>(this IUnitOfWork uow, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            var entities = await uow.GetRepository<T>().GetAllAsync(includes);
            return entities.ToList();
        }
    }
}
