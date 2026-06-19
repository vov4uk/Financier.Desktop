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
            Dictionary<string, List<string>> entityColumnsOrder,
            bool deleteRawFile = true)
        {
            using var writer = new StreamWriter(Path.GetFileNameWithoutExtension(fileName));

            WriteHeader(writer, backupVersion);
            WriteBody(writer, entities, entityColumnsOrder);
            WriteFooter(writer);
            writer.Flush();
            writer.Close();
            var fileWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            await Compress(fileWithoutExt, fileName);
            if (deleteRawFile && File.Exists(fileWithoutExt))
            {
                File.Delete(fileWithoutExt);
            }
        }

        private void WriteHeader(TextWriter bw, BackupVersion backupVersion)
        {
            bw.WriteLine($"{Backup.PACKAGE}:{backupVersion.Package}");
            bw.WriteLine($"{Backup.VERSION_CODE}:{backupVersion.VersionCode}");
            bw.WriteLine($"{Backup.VERSION_NAME}:{backupVersion.Version}");
            bw.WriteLine($"{Backup.DATABASE_VERSION}:{backupVersion.DatabaseVersion++}");
            bw.WriteLine(Backup.START);
        }

        private void WriteBody(TextWriter bw, IEnumerable<Entity> entities, Dictionary<string, List<string>> columnsOrder)
        {
            var byType = entities.ToLookup(e => e.GetType());
            foreach (Type type in ExportOrder)
            {
                ExportTable(bw, byType[type], columnsOrder);
            }
        }

        private void WriteFooter(TextWriter bw)
        {
            bw.Write(Backup.END);
        }

        private void ExportTable(TextWriter bw, IEnumerable<Entity> ent, Dictionary<string, List<string>> entityColumnsOrder)
        {
            foreach (Entity item in ent)
            {
                bw.Write(item.ToBackupLines(entityColumnsOrder));
            }
        }

        public static string GenerateFileName()
        {
            return DateTime.Now.ToString("yyyyMMdd'_'HHmmss'_'fff") + ".backup";
        }

        private async Task Compress(string sourceFile, string compressedFile)
        {
            using FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate);
            using FileStream targetStream = File.Create(compressedFile);
            using GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
            await sourceStream.CopyToAsync(compressionStream);
            compressionStream.Flush();
            compressionStream.Close();
        }
    }
}
