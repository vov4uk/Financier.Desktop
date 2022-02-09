using System.Collections.Generic;
using System.Linq;

namespace Financier.Common.Model
{
    public interface IActive
    {
        public long? Id { get; set; }

        bool IsActive { get; }

        string Title { get; set; }
    }

    public static class IEnumerableExtentions
    {
        public static IEnumerable<T> DefaultOrder<T>(this IEnumerable<T> collection)
            where T : IActive
        {
            return collection.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id);
        }
    }
}
