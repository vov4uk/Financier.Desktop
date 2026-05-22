namespace Financier.Desktop.Tests.ViewModel
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Financier.Adapter;
    using Financier.DataAccess;
    using Financier.Desktop.Helpers;
    using Financier.Desktop.Helpers.BankHelper;
    using Financier.Desktop.ViewModel;
    using Moq;
    using Xunit;

    [Collection("Integration tests")]
    [CollectionDefinition("Integration tests", DisableParallelization = true)]
    public class FinancierVMIntegrationTest
    {
        [Fact]
        public async Task OpenBackup_ParseBackup_ImportEntities()
        {
            var dialogMock = new Mock<IDialogWrapper>();
            dialogMock.Setup(x => x.ShowMessageBox(It.IsAny<string>(), "Success", false)).Returns(true);
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");
            var vm = new MainWindowVM(dialogMock.Object, new FinancierDatabaseFactory(), new EntityReader(), null, null, null);

            await vm.OpenBackup(backupPath);

            Assert.True(vm.CurrentPage is BlotterVM);
            Assert.Equal(backupPath, vm.OpenBackupPath);
        } 

        [Fact]
        public async Task SaveBackup_OpenBackup_ShouldSaveSameBackup()
        {
            var dialogMock = new Mock<IDialogWrapper>();
            dialogMock.Setup(x => x.ShowMessageBox(It.IsAny<string>(), "Success", false)).Returns(true);
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");
            var vm = new MainWindowVM(dialogMock.Object, new FinancierDatabaseFactory(), new EntityReader(), new BackupWriter(), null, new BankHelperFactory());

            await vm.OpenBackup(backupPath);

            var newBackupPath = Path.Combine(Path.GetDirectoryName(backupPath), BackupWriter.GenerateFileName());

            await vm.SaveBackup(newBackupPath);

            Assert.True(File.Exists(newBackupPath));
            File.Delete(newBackupPath);
        }
    }
}
