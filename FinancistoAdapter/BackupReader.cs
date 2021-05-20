using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace FinancistoAdapter
{
    public class BackupReader : IDisposable, IBackup
    {
        private readonly FileStream _file;
        private readonly GZipStream _zipStream;
        private TextReader _reader;

        private string _package;
        private int _versionCode;
        private Version _version;
        private int _dbVersion;

        public string Package { get { return _package; } }
        public int VersionCode { get { return _versionCode; } }
        public Version Version { get { return _version; } }
        public int DatabaseVersion { get { return _dbVersion; } }

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
                        _package = line.Value; 
                        break;
                    case Backup.VERSION_CODE:
                        _versionCode = int.Parse(line.Value);
                        break;
                    case Backup.VERSION_NAME:
                        _version = Version.Parse(line.Value);
                        break;
                    case Backup.DATABASE_VERSION:
                        _dbVersion = int.Parse(line.Value);
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
            _reader.Dispose();
            _zipStream.Dispose();
            _file.Dispose();
        }
    }
}
