using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public class Line
	{
		public Line(string rawLine)
		{
			if (!String.IsNullOrEmpty(rawLine))
			{
				string[] splitted = rawLine.Split(new[] {':'}, 2);
				Key = splitted[0];
				if (splitted.Length > 1) 
					Value = splitted[1];
			}
		}

		public Line(string key, string value)
		{
			Key = key;
			Value = value;
		}

		public string Key { get; set; }
		public string Value { get; set; }
	}

	public class BackupReader : IDisposable
	{
		private readonly FileStream _file;
		private readonly GZipStream _zipStream;
		private TextReader _reader;
		private long _startPos = 0;

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
			if (String.IsNullOrEmpty(fileName)) throw new ArgumentException("File name cannot be null or empty.", "fileName");
			_file = File.OpenRead(fileName);
			_zipStream = new GZipStream(_file, CompressionMode.Decompress);
			_reader = new StreamReader(_zipStream);

			ReadHeader();
		}

		private void ReadHeader()
		{
			string rawLine;
			while ((rawLine = _reader.ReadLine()) != null && !String.Equals(rawLine, "#START"))
			{
				Line line = new Line(rawLine);
				switch (line.Key)
				{
					case "PACKAGE": 
						_package = line.Value; 
						break;
					case "VERSION_CODE":
						_versionCode = int.Parse(line.Value);
						break;
					case "VERSION_NAME":
						_version = Version.Parse(line.Value);
						break;
					case "DATABASE_VERSION":
						_dbVersion = int.Parse(line.Value);
						break;
				}
			}

			_startPos = _file.Position;
		}

		public IEnumerable<string> GetLines()
		{
			_file.Seek(_startPos, SeekOrigin.Begin);
			_reader = new StreamReader(_zipStream);
			string line;
			while ((line = _reader.ReadLine()) != null && line != "#END")
			{
				if (!String.IsNullOrEmpty(line))
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
