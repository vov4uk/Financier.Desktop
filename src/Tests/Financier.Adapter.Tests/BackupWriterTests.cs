namespace Financier.Adapter.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Financier.Adapter.Tests.Common;
    using Financier.DataAccess.Data;
    using Xunit;

    public class BackupWriterTests
    {
        [Theory]
        [AutoMoqData]
        public void GenerateBackup_ArchiveTransactions_FileExist(string fileName, BackupVersion version, List<Transaction> transactions)
        {
            EntityReader.EntityColumnsOrder.Clear();
            EntityReader.EntityColumnsOrder.Add("transactions", PredefinedData.TransactionsColumnsOrder);

            string path = fileName + ".backup";

            BackupWriter writer = new BackupWriter(path, version);

            writer.GenerateBackup(new List<Entity>(transactions));

            Assert.True(File.Exists(path));
            File.Delete(path);
        }
    }
}