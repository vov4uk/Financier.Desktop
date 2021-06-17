using Financier.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Financier.Adapter
{
    public class BackupWriter : IDisposable
    {
        private readonly TextWriter _writer;
        private readonly string _fileName;

        public BackupVersion BackupVersion { get; }

        public BackupWriter(string fileName, BackupVersion backupVersion)
        {
            BackupVersion = backupVersion;
            _fileName = fileName;
            _writer = new StreamWriter(Path.GetFileNameWithoutExtension(_fileName));
        }

        public void GenerateBackup(List<Entity> entities)
        {
            WriteHeader(_writer);
            WriteBody(_writer, entities);
            WriteFooter(_writer);
            _writer.Flush();
            _writer.Close();
            var fileWithoutExt = Path.GetFileNameWithoutExtension(_fileName);
            Compress(fileWithoutExt, _fileName);
            if (!Debugger.IsAttached && File.Exists(fileWithoutExt))
            {
                File.Delete(fileWithoutExt);
            }
        }

        private void WriteHeader(TextWriter bw)
        {
            bw.WriteLine($"{Backup.PACKAGE}:{BackupVersion.Package}");
            bw.WriteLine($"{Backup.VERSION_CODE}:{BackupVersion.VersionCode}");
            bw.WriteLine($"{Backup.VERSION_NAME}:{BackupVersion.Version}");
            bw.WriteLine($"{Backup.DATABASE_VERSION}:{BackupVersion.DatabaseVersion}");
            bw.WriteLine(Backup.START);
        }

        private void WriteBody(TextWriter bw, List<Entity> entities)
        {
            ExportTable(bw, entities.OfType<Account>().ToArray());
            ExportTable(bw, entities.OfType<AttributeDefinition>().ToArray());
            ExportTable(bw, entities.OfType<CategoryAttribute>().ToArray());
            ExportTable(bw, entities.OfType<TransactionAttribute>().ToArray());
            ExportTable(bw, entities.OfType<Budget>().ToArray());
            ExportTable(bw, entities.OfType<Category>().ToArray());
            ExportTable(bw, entities.OfType<Currency>().ToArray());
            ExportTable(bw, entities.OfType<Location>().ToArray());
            ExportTable(bw, entities.OfType<Project>().ToArray());
            ExportTable(bw, entities.OfType<Transaction>().ToArray());
            ExportTable(bw, entities.OfType<Payee>().ToArray());
            ExportTable(bw, entities.OfType<CCardClosingDate>().ToArray());
            ExportTable(bw, entities.OfType<SmsTemplate>().ToArray());
            ExportTable(bw, entities.OfType<CurrencyExchangeRate>().ToArray());
        }

        private void WriteFooter(TextWriter bw)
        {
            bw.Write(Backup.END);
        }

        private void ExportTable<T>(TextWriter bw, T[] ent) where T : Entity
        {
            if (ent.Length > 0)
            {
                foreach (var item in ent)
                {
                    bw.Write(item.ToBackupLines());
                }
            }
        }

        public static string GenerateFileName()
        {
            return DateTime.Now.ToString("yyyyMMdd'_'HHmmss'_'fff") + ".backup";
        }

        public void Dispose()
        {
            _writer.Dispose();
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