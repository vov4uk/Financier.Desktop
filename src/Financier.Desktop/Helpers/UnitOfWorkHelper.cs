using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Financier.Desktop.Helpers
{
    internal static class UnitOfWorkHelper
    {
        public static async Task<ObservableCollection<T>> GetAllOrderedAsync<T>(this IUnitOfWork uow, Func<T, object> orderDescBy = null, Func<T, object> thenBy = null, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            var entities = await uow.GetRepository<T>().GetAllAsync(includes);
            if (orderDescBy != null)
            {
                if (thenBy != null)
                {
                    return new ObservableCollection<T>(entities.OrderByDescending(orderDescBy).ThenBy(thenBy));
                }
                else
                {
                    return new ObservableCollection<T>(entities.OrderByDescending(orderDescBy));
                }
            }
            return new ObservableCollection<T>(entities);
        }

        public static async Task<ObservableCollection<T>> GetAllAsync<T>(this IUnitOfWork uow, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            var entities = await uow.GetRepository<T>().GetAllAsync(includes);
            return new ObservableCollection<T>(entities);
        }

        public static async Task<ObservableCollection<T>> GetAllOrderedByDefaultAsync<T>(this IUnitOfWork uow, params Expression<Func<T, object>>[] includes)
            where T : class, IActive
        {
            var entities = await uow.GetRepository<T>().GetAllAsync(includes);
            return new ObservableCollection<T>(entities.OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
        }
    }
}
