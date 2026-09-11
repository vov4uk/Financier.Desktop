namespace Financier.Adapter.Tests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Xunit;

    public class BackupReaderTests
    {
        [Fact]
        public async Task GetLines_ReadLinesFromArchive_ReadCorrectCount()
        {
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");

            using BackupReader backupReader = new BackupReader(backupPath);

            int count = 0;
            await foreach (var _ in backupReader.GetLinesAsync())
            {
                count++;
            }

            Assert.Equal(343, count);
            Assert.Equal(211, backupReader.BackupVersion.DatabaseVersion);
            Assert.Equal("ru.orangesoftware.financisto", backupReader.BackupVersion.Package);
            Assert.Equal("1.7.4", backupReader.BackupVersion.Version);
            Assert.Equal(100, backupReader.BackupVersion.VersionCode);
        }
    }
}
