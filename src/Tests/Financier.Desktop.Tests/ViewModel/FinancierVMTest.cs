namespace Financier.Desktop.Tests.ViewModel
{
    using Financier.DataAccess.Data;
    using Financier.DataAccess.View;
    using Financier.Desktop.ViewModel;
    using Xunit;

    public class FinancierVMTest
    {
        [Fact]
        public void Constructor_NoParameters_PagesCreated()
        {
            var vm = new FinancierVM(null);

            Assert.NotNull(vm.Blotter);
            Assert.NotNull(vm.Locations);
            Assert.NotNull(vm.Payees);
            Assert.NotNull(vm.Projects);
            Assert.Equal(10, vm.Pages.Count);
        }

        [Fact]
        public void MenuNavigateCommand_ChangeCurrentPage_PropertiesUpdated()
        {
            var vm = new FinancierVM(null);

            vm.MenuNavigateCommand.Execute(typeof(BlotterTransactions));
            Assert.True(vm.IsTransactionPageSelected);
            vm.MenuNavigateCommand.Execute(typeof(Location));
            Assert.True(vm.IsLocationPageSelected);
            vm.MenuNavigateCommand.Execute(typeof(Project));
            Assert.True(vm.IsProjectPageSelected);
            vm.MenuNavigateCommand.Execute(typeof(Payee));
            Assert.True(vm.IsPayeePageSelected);
            vm.MenuNavigateCommand.Execute(typeof(CurrencyExchangeRate));
            Assert.True(vm.CurrentPage is ExchangeRatesVM);
        }
    }
}
