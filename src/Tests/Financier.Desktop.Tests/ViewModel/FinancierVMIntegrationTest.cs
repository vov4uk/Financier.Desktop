namespace Financier.Desktop.Tests.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Financier.Adapter;
    using Financier.DataAccess;
    using Financier.DataAccess.Data;
    using Financier.Desktop.ViewModel;
    using Financier.Tests.Common;
    using Xunit;

    public class FinancierVMIntegrationTest
    {
        [Fact]
        public async void OpenBackup_ParseBackup_ImportEntities()
        {
            var backupPath = Path.Combine(Environment.CurrentDirectory, "Assets", "min.backup");
            var vm = new FinancierVM(null, new FinancierDatabaseFactory(), new EntityReader(), null);

            await vm.OpenBackup(backupPath);

            Assert.Equal(3, vm.Pages.OfType<AccountsVM>().First().Entities.Count);
            Assert.Equal(6, vm.Pages.OfType<CategoriesVM>().First().Entities.Count);
            Assert.Equal(5, vm.Pages.OfType<BlotterVM>().First().Entities.Count);
            Assert.Single(vm.Pages.OfType<CurrenciesVM>().First().Entities);
            Assert.True(vm.CurrentPage is InfoVM);
            Assert.Equal(backupPath, vm.OpenBackupPath);
        }
    }
}
