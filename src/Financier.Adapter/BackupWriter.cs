using Financier.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Adapter
{
    public class BackupWriter : IBackupWriter
    {
        private static readonly Type[] ExportOrder =
        [
            typeof(Account),
            typeof(AttributeDefinition),
            typeof(CategoryAttribute),
            typeof(TransactionAttribute),
            typeof(Budget),
            typeof(Category),
            typeof(Currency),
            typeof(Location),
            typeof(Project),
            typeof(Transaction),
            typeof(Payee),
            typeof(CCardClosingDate),
            typeof(SmsTemplate),
            typeof(CurrencyExchangeRate),
        ];

        public async Task GenerateBackupAsync(
            IEnumerable<Entity> entities,
            string fileName,
            BackupVersion backupVersion,
            Dictionary<string, List<string>> entityColumnsOrder)
        {
            using var fileStream = File.Create(fileName);
            using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
            using var writer = new StreamWriter(gzipStream);

            WriteHeader(writer, backupVersion);
            WriteBody(writer, entities, entityColumnsOrder);
            WriteFooter(writer);
            await writer.FlushAsync();
        }

        private static void WriteHeader(TextWriter bw, BackupVersion backupVersion)
        {
            bw.WriteLine($"{Backup.PACKAGE}:{backupVersion.Package}");
            bw.WriteLine($"{Backup.VERSION_CODE}:{backupVersion.VersionCode}");
            bw.WriteLine($"{Backup.VERSION_NAME}:{backupVersion.Version}");
            bw.WriteLine($"{Backup.DATABASE_VERSION}:{backupVersion.DatabaseVersion++}");
            bw.WriteLine(Backup.START);
        }

        private static void WriteBody(TextWriter bw, IEnumerable<Entity> entities, Dictionary<string, List<string>> columnsOrder)
        {
            var columnData = BuildColumnData(columnsOrder);
            var byType = entities.ToLookup(e => e.GetType());
            foreach (Type type in ExportOrder)
            {
                foreach (Entity item in byType[type])
                    item.WriteBackupLines(bw, columnData);
            }
        }

        private static void WriteFooter(TextWriter bw)
        {
            bw.Write(Backup.END);
        }

        private static Dictionary<string, (Dictionary<string, int> Index, int Count)> BuildColumnData(
            Dictionary<string, List<string>> columnsOrder)
        {
            var result = new Dictionary<string, (Dictionary<string, int>, int)>(columnsOrder.Count);
            foreach (var (table, cols) in columnsOrder)
            {
                var index = new Dictionary<string, int>(cols.Count);
                for (int i = 0; i < cols.Count; i++)
                    index[cols[i]] = i;
                result[table] = (index, cols.Count);
            }
            return result;
        }

        public static string GenerateFileName()
        {
            return DateTime.Now.ToString("yyyyMMdd'_'HHmmss'_'fff") + ".backup";
        }
    }
}
