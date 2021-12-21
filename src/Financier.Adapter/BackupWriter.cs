using Financier.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Financier.Adapter
{
    public class BackupWriter : IBackupWriter
    {
        public void GenerateBackup(
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
            Compress(fileWithoutExt, fileName);
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
            ExportTable(bw, entities.OfType<Account>(), columnsOrder);
            ExportTable(bw, entities.OfType<AttributeDefinition>(), columnsOrder);
            ExportTable(bw, entities.OfType<CategoryAttribute>(), columnsOrder);
            ExportTable(bw, entities.OfType<TransactionAttribute>(), columnsOrder);
            ExportTable(bw, entities.OfType<Budget>(), columnsOrder);
            ExportTable(bw, entities.OfType<Category>(), columnsOrder);
            ExportTable(bw, entities.OfType<Currency>(), columnsOrder);
            ExportTable(bw, entities.OfType<Location>(), columnsOrder);
            ExportTable(bw, entities.OfType<Project>(), columnsOrder);
            ExportTable(bw, entities.OfType<Transaction>(), columnsOrder);
            ExportTable(bw, entities.OfType<Payee>(), columnsOrder);
            ExportTable(bw, entities.OfType<CCardClosingDate>(), columnsOrder);
            ExportTable(bw, entities.OfType<SmsTemplate>(), columnsOrder);
            ExportTable(bw, entities.OfType<CurrencyExchangeRate>(), columnsOrder);
        }

        private void WriteFooter(TextWriter bw)
        {
            bw.Write(Backup.END);
        }

        private void ExportTable<T>(TextWriter bw, IEnumerable<T> ent, Dictionary<string, List<string>> _entityColumnsOrder) where T : Entity
        {
            foreach (var item in ent)
            {
                bw.Write(item.ToBackupLines(_entityColumnsOrder));
            }
        }

        public static string GenerateFileName()
        {
            return DateTime.Now.ToString("yyyyMMdd'_'HHmmss'_'fff") + ".backup";
        }

        public void Compress(string sourceFile, string compressedFile)
        {
            using FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate);
            using FileStream targetStream = File.Create(compressedFile);
            using GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
            sourceStream.CopyTo(compressionStream);
            compressionStream.Flush();
            compressionStream.Close();
        }
    }
}