using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financier.Common.Model;
using Financier.Common.Utils;
using Xunit;

namespace Financier.Converters.Tests
{
    public class BlotterUtilsTests
    {
        [Fact]
        public void SetAmountText_ShouldReturnCorrectString_WhenCurrencyModelIsPassed()
        {
            // Arrange
            CurrencyModel c = new CurrencyModel { Name = "USD", Symbol = "$" };
            long amount = 100;
            bool addPlus = true;
            string expected = "+1.00 $";

            // Act
            string actual = BlotterUtils.SetAmountText(c, amount, addPlus);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SetAmountText_ShouldReturnCorrectString_WhenCurrencyModelIsNotPassed()
        {
            // Arrange
            long amount = 100;
            bool addPlus = true;
            string expected = "+1.00";

            // Act
            string actual = BlotterUtils.SetAmountText(null, amount, addPlus);

            // Assert
            Assert.Equal(expected, actual);
        }

    }
}
