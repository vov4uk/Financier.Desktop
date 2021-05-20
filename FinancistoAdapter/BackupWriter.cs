using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
{
    public class BackupWriter : IDisposable, IBackup
    {
        private TextWriter _writer;
        private string _fileName;

        private string _package;
        private int _versionCode;
        private Version _version;
        private int _dbVersion;

        public BackupWriter(string package, int versionCode, Version version, int dbVersion)
        {
            _package = package;
            _versionCode = versionCode;
            _version = version;
            _dbVersion = dbVersion;
            _fileName = "c:\\" + generateFilename();
            _writer = new StreamWriter(_fileName);
        }

        public string Package
        {
            get { return _package; }
        }

        public int VersionCode
        {
            get { return _versionCode; }
        }

        public Version Version
        {
            get { return _version; }
        }

        public int DatabaseVersion
        {
            get { return _dbVersion; }
        }


        public void GenerateBackup(Entity[] entities)
        {
            WriteHeader(_writer);
            WriteBody(_writer, entities);
            WriteFooter(_writer);
            _writer.Flush();
            _writer.Close();
            Compress(_fileName, _fileName + ".backup");
            File.Delete(_fileName);
        }

        private void WriteHeader(TextWriter bw)
        {
            bw.WriteLine($"{Backup.PACKAGE}:{Package}");
            bw.WriteLine($"{Backup.VERSION_CODE}:{VersionCode}");
            bw.WriteLine($"{Backup.VERSION_NAME}:{Version}");
            bw.WriteLine($"{Backup.DATABASE_VERSION}:{DatabaseVersion}");
            bw.WriteLine(Backup.START);
        }

        private void WriteBody(TextWriter bw, Entity[] entities)
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
            ExportTable(bw, entities.OfType<ExchangeRate>().ToArray());

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

        public String generateFilename()
        {
            return DateTime.Now.ToString("yyyyMMdd'_'HHmmss'_'fff");
        }


        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Compress(string sourceFile, string compressedFile)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                using (FileStream targetStream = File.Create(compressedFile))
                {
                    using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }
        }
    }
}