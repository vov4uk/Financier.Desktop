namespace Financier.Adapter.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Xunit;

    public class BackupReaderTests
    {
        [Fact]
        public void GetLines_ReadLinesFromArchive_ReadCorrectCount()
        {
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");

            using BackupReader backupReader = new BackupReader(backupPath);

            var lines = backupReader.GetLines();

            Assert.Equal(343, lines.Count());
            Assert.Equal(211, backupReader.BackupVersion.DatabaseVersion);
            Assert.Equal("ru.orangesoftware.financisto", backupReader.BackupVersion.Package);
            Assert.Equal(new Version(1, 7, 4), backupReader.BackupVersion.Version);
            Assert.Equal(100, backupReader.BackupVersion.VersionCode);
        }
    }
}