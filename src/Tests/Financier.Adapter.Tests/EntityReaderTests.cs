namespace Financier.Adapter.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using Financier.DataAccess.Data;
    using Financier.Tests.Common;
    using Xunit;

    public class EntityReaderTests
    {
        [Fact]
        public void ParseBackupFile_ReadEntitiesFromArchive_ReadCorrectCount()
        {
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");

            var (entities, backupVersion, columnsOrder) = EntityReader.ParseBackupFile(backupPath);

            Assert.Equal(211, backupVersion.DatabaseVersion);
            Assert.Equal("ru.orangesoftware.financisto", backupVersion.Package);
            Assert.Equal(new Version(1, 7, 4), backupVersion.Version);
            Assert.Equal(100, backupVersion.VersionCode);

            Assert.Equal(3, entities.OfType<Account>().Count());
            Assert.Equal(7, entities.OfType<Category>().Count());
            Assert.Equal(7, entities.OfType<Transaction>().Count());
            Assert.Single(entities.OfType<Currency>());
            Assert.Equal(2, entities.OfType<SmsTemplate>().Count());
            Assert.Equal(PredefinedData.TransactionsColumnsOrder, columnsOrder["transactions"]);
            Assert.Contains(PredefinedData.Transaction, entities.OfType<Transaction>(), new TestComparer());
        }
    }
}
