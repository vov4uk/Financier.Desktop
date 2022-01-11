using Financier.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

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

        private void ReadHeader()
        {
            string rawLine;
            while ((rawLine = _reader.ReadLine()) != null && !string.Equals(rawLine, Backup.START))
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
                        BackupVersion.Version = Version.Parse(line.Value);
                        break;

                    case Backup.DATABASE_VERSION:
                        BackupVersion.DatabaseVersion = int.Parse(line.Value);
                        break;
                }
            }
        }

        public IEnumerable<string> GetLines()
        {
            _reader = new StreamReader(_zipStream);
            _file.Seek(0, SeekOrigin.Begin);
            ReadHeader();

            string line;
            while ((line = _reader.ReadLine()) != null && line != Backup.END)
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