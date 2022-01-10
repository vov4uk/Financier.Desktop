using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Adapter
{
    public interface IBackupWriter
    {
        void GenerateBackup(
            IEnumerable<Entity> entities,
            string fileName,
            BackupVersion backupVersion,
            Dictionary<string, List<string>> entityColumnsOrder,
            bool deleteRawFile = true);
    }
}
