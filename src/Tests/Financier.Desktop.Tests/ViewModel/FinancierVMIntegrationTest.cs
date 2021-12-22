namespace Financier.Desktop.Tests.ViewModel
{
    using System;
    using System.IO;
    using Financier.Adapter;
    using Financier.DataAccess;
    using Financier.Desktop.ViewModel;
    using Xunit;

    public class FinancierVMIntegrationTest
    {
        [Fact]
        public async void OpenBackup_ParseBackup_ImportEntities()
        {
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");
            var vm = new FinancierVM(null, new FinancierDatabaseFactory(), new EntityReader(), null);

            await vm.OpenBackup(backupPath);

            Assert.True(vm.CurrentPage is BlotterVM);
            Assert.Equal(backupPath, vm.OpenBackupPath);
        }

        [Fact]
        public async void SaveBackup_OpenBackup_ShouldSaveSameBackup()
        {
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");
            var vm = new FinancierVM(null, new FinancierDatabaseFactory(), new EntityReader(), new BackupWriter());

            await vm.OpenBackup(backupPath);

            var newBackupPath = Path.Combine(Path.GetDirectoryName(backupPath), BackupWriter.GenerateFileName());

            await vm.SaveBackup(newBackupPath);

            Assert.True(File.Exists(newBackupPath));
            File.Delete(newBackupPath);
        }
    }
}
