namespace Financier.Converters.Tests
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using AutoFixture.Xunit2;
    using Financier.DataAccess.Data;
    using Financier.Converters;
    using Xunit;

    public class AmountTextConverterTest
    {
        private readonly IMultiValueConverter converter = new AmountTextConverter();

        [InlineAutoData(new object[] { })]
        [InlineAutoData(null)]
        [Theory]
        public void Convert_InvalidParameters_NotApplicable(object[] values)
        {
            // Act
            var actual = converter.Convert(values, null, null, CultureInfo.InvariantCulture);

            Assert.Equal("N/A", actual);
        }

        [InlineAutoData(new object[] { "xxx", null }, "xxx")]
        [Theory]
        public void Convert_NotNumericParameters_ToString(object[] values, string expexted)
        {
            // Act
            var actual = converter.Convert(values, null, null, CultureInfo.InvariantCulture);

            Assert.Equal(expexted, actual);
        }

        [Fact]
        public void Convert_NumericParameters_ExpectedString()
        {
            var currency = new Currency() { Id = 1, Title = "Dollar", IsDefault = true, IsActive = true, Name = "USD", Decimals = 2, Symbol = "$", SymbolFormat = "." };

            // Act
            var actual = converter.Convert(new object[] { "100", currency }, null, null, CultureInfo.InvariantCulture);

            Assert.Equal("1.00 $", actual);
        }

        [Fact]
        public void ConvertBack_NotImplemented_ThorwsException()
        {
            // Act
            Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, null, null, CultureInfo.InvariantCulture));
        }
    }
}
