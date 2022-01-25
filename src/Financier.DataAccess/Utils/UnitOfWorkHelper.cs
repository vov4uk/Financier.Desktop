using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Financier.DataAccess.Utils
{
    public static class UnitOfWorkHelper
    {
        public static async Task<List<T>> GetAllAsync<T>(this IUnitOfWork uow, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            var entities = await uow.GetRepository<T>().GetAllAsync(includes);
            return entities.ToList();
        }

        public static async Task<List<T>> GetAllOrderedByDefaultAsync<T>(this IUnitOfWork uow, params Expression<Func<T, object>>[] includes)
            where T : class, IActive
        {
            var entities = await uow.GetRepository<T>().GetAllAsync(includes);
            return entities.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id).ToList();
        }
    }
}
