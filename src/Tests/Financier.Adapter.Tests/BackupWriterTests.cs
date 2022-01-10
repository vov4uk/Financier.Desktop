namespace Financier.Adapter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Financier.DataAccess.Data;
    using Financier.Tests.Common;
    using Xunit;

    public class BackupWriterTests
    {
        [Theory]
        [AutoMoqData]
        public void GenerateBackup_ArchiveTransactions_FileExist(string fileName, BackupVersion version, List<Transaction> transactions)
        {
            Dictionary<string, List<string>> entityColumnsOrder = new Dictionary<string, List<string>>();
            entityColumnsOrder.Add("transactions", PredefinedData.TransactionsColumnsOrder);

            string path = fileName + ".backup";

            BackupWriter writer = new BackupWriter();

            writer.GenerateBackup(new List<Entity>(transactions), path, version, entityColumnsOrder);

            Assert.True(File.Exists(path));
            File.Delete(path);
        }

        [Fact]
        public void GenerateBackup_ParseBackup_CompareGeneratedFileWithRaw()
        {
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");
            var expectedTextPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min");
            var actualPath = Path.Combine(Environment.CurrentDirectory, "Assets", "actual.backup");
            var fileWithoutExt = Path.GetFileNameWithoutExtension(actualPath);

            var reader = new EntityReader();
            var (entities, backupVersion, columnsOrder) = reader.ParseBackupFile(backupPath);

            BackupWriter writer = new BackupWriter();

            writer.GenerateBackup(entities, actualPath, backupVersion, columnsOrder, false);

            var actualText = File.ReadAllText(fileWithoutExt);
            var expectedText = File.ReadAllText(expectedTextPath);

            Assert.True(File.Exists(actualPath));
            Assert.Equal(actualText, expectedText);

            File.Delete(actualPath);
            File.Delete(fileWithoutExt);
        }
    }
}