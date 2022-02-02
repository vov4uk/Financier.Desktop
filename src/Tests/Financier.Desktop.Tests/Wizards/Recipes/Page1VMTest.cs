namespace Financier.Desktop.Tests.Wizards.Recipes
{
    using Financier.Desktop.Helpers;
    using Financier.Desktop.Wizards.RecipesWizard.ViewModel;
    using Xunit;

    public class Page1VMTest
    {
        [Fact]
        public void Constructor_NoParameters_ObjectCreated()
        {
            var vm = new Page1VM(-100.0);

            Assert.Equal("Paste text", vm.Title);
            Assert.Equal(-100.0, vm.TotalAmount);
            Assert.False(vm.IsValid());
            Assert.Empty(vm.Amounts);
        }

        [Fact]
        public void CalculateCurrentAmount_ParseText_ReceiveAmounts()
        {
            var vm = new Page1VM(-100.0);
            vm.Text = @"
Beer Baltyka 0 50,0 A
Milk Molokia 2.5% 25.0 A
25.50 A
100500.0
100600
Discount -0.5 A";
            vm.CalculateCurrentAmount();

            Assert.Equal(-100.0, vm.TotalAmount);
            Assert.Equal(-100.0, vm.CalculatedAmount);
            Assert.Equal(0.0, vm.Diff);
            Assert.Equal(4, vm.Amounts.Count);
            Assert.True(vm.IsValid());
        }

        [Fact]
        public void RecipiesHelper_FormatText_TextFormated()
        {
            var text = @"
Beer Baltyka 0 50,0 A
Milk Molokia 2.5% 25.0 а                         
Milk Molokia 2.5%
25.50 A



100500.0
100600
Discount
-0.5-б 
Total
100500";
            string expected = @"Beer Baltyka 0 50,0-A
Milk Molokia 2.5% 25.0-а
Milk Molokia 2.5% 25.50-A
100500.0 100600 Discount -0.5-б
Total 100500";

            var actual = RecipiesHelper.FormatText(text);

            Assert.Equal(expected, actual);
        }
    }
}
