namespace Financier.Adapter.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Financier.Adapter.Tests.Common;
    using Financier.DataAccess.Data;
    using Xunit;

    public class EntityReaderTests
    {
        [Fact]
        public void ParseBackupFile_ReadEntitiesFromArchive_ReadCorrectCount()
        {
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");

            var entities = EntityReader.ParseBackupFile(backupPath);

            Assert.Equal(211, EntityReader.BackupVersion.DatabaseVersion);
            Assert.Equal("ru.orangesoftware.financisto", EntityReader.BackupVersion.Package);
            Assert.Equal(new Version(1, 7, 4), EntityReader.BackupVersion.Version);
            Assert.Equal(100, EntityReader.BackupVersion.VersionCode);

            Assert.Equal(3, entities.OfType<Account>().Count());
            Assert.Equal(7, entities.OfType<Category>().Count());
            Assert.Equal(7, entities.OfType<Transaction>().Count());
            Assert.Single(entities.OfType<Currency>());
            Assert.Equal(2, entities.OfType<SmsTemplate>().Count());
            Assert.Equal(PredefinedData.TransactionsColumnsOrder, EntityReader.EntityColumnsOrder["transactions"]);
            Assert.Contains(PredefinedData.Transaction, entities.OfType<Transaction>(), new TestComparer());
        }
    }
}
