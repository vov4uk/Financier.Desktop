using System.Collections.Generic;
using System.Threading.Tasks;
using Financier.DataAccess.Data;

namespace Financier.Adapter
{
    public interface IBackupWriter
    {
        Task GenerateBackupAsync(
            IEnumerable<Entity> entities,
            string fileName,
            BackupVersion backupVersion,
            Dictionary<string, List<string>> entityColumnsOrder);
    }
}
