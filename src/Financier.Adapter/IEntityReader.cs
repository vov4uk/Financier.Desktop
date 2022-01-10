using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Adapter
{
    public interface IEntityReader
    {
        (IEnumerable<Entity> Entities, BackupVersion BackupVersion, Dictionary<string, List<string>> EntityColumnsOrder) ParseBackupFile(string fileName);
    }
}
