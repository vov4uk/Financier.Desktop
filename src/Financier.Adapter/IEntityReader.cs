using System.Collections.Generic;
using System.Threading.Tasks;
using Financier.DataAccess.Data;

namespace Financier.Adapter
{
    public interface IEntityReader
    {
        Task<(IEnumerable<Entity> Entities, BackupVersion BackupVersion, Dictionary<string, List<string>> EntityColumnsOrder)> ParseBackupFileAsync(string fileName);
    }
}
