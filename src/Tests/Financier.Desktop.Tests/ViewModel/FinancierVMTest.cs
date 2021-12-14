namespace Financier.Desktop.Tests.ViewModel
{
    using Financier.Desktop.ViewModel;
    using Financier.Tests.Common;
    using System.Threading.Tasks;
    using Xunit;

    public class FinancierVMTest
    {
        [Fact]
        public void Constructor_NoParameters_PagesCreated()
        {
            var vm = new FinancierVM();

            Assert.NotNull(vm.Blotter);
            Assert.NotNull(vm.Locations);
            Assert.NotNull(vm.Payees);
            Assert.NotNull(vm.Projects);
            Assert.Equal(10, vm.Pages.Count);
        }
    }
}
