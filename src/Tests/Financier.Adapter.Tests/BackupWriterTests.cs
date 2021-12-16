namespace Financier.Adapter.Tests
{
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

            BackupWriter writer = new BackupWriter(path, version, entityColumnsOrder);

            writer.GenerateBackup(new List<Entity>(transactions));

            Assert.True(File.Exists(path));
            File.Delete(path);
        }
    }
}