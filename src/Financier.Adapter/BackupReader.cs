using Financier.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Financier.Adapter
{
    public class BackupReader : IDisposable
    {
        private readonly FileStream _file;
        private readonly GZipStream _zipStream;
        private readonly BackupVersion backupVersion = new BackupVersion();
        private TextReader _reader;
        private bool isDisposed;

        public BackupVersion BackupVersion => backupVersion;
        public BackupReader(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("File name cannot be null or empty.", "fileName");
            _file = File.OpenRead(fileName);
            _zipStream = new GZipStream(_file, CompressionMode.Decompress);
        }

        private async Task ReadHeaderAsync(CancellationToken cancellationToken = default)
        {
            string rawLine;
            while ((rawLine = (await _reader.ReadLineAsync(cancellationToken))!) != null && !string.Equals(rawLine, Backup.START))
            {
                Line line = new Line(rawLine);
                switch (line.Key)
                {
                    case Backup.PACKAGE:
                        BackupVersion.Package = line.Value;
                        break;

                    case Backup.VERSION_CODE:
                        BackupVersion.VersionCode = int.Parse(line.Value);
                        break;

                    case Backup.VERSION_NAME:
                        BackupVersion.Version = line.Value;
                        break;

                    case Backup.DATABASE_VERSION:
                        BackupVersion.DatabaseVersion = int.Parse(line.Value);
                        break;
                }
            }
        }

        public async IAsyncEnumerable<string> GetLinesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _reader = new StreamReader(_zipStream);
            _file.Seek(0, SeekOrigin.Begin);
            await ReadHeaderAsync(cancellationToken);

            string line;
            while ((line = (await _reader.ReadLineAsync(cancellationToken))!) != null && line != Backup.END)
            {
                if (!string.IsNullOrEmpty(line))
                    yield return line;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _reader?.Dispose();
                _zipStream?.Dispose();
                _file?.Dispose();
            }

            this.isDisposed = true;
        }
    }
}
