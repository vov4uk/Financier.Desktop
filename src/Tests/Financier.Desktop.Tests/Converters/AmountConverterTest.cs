namespace Financier.Desktop.Tests.Converters
{
    using System.Globalization;
    using System.Windows.Data;
    using AutoFixture.Xunit2;
    using Financier.Desktop.Converters;
    using Xunit;

    public class AmountConverterTest
    {
        private const long TenThousand = 10000L;
        private const long MinusTenThousand = -10000L;
        private readonly IValueConverter converter = new AmountConverter();

        [InlineAutoData(-100, "false", -1.0)]
        [InlineAutoData(-100, "true", 1.0)]
        [InlineAutoData(-100, "qwerty", 1.0)]
        [InlineAutoData(100, "false", 1.0)]
        [InlineAutoData(100, "true", 1.0)]
        [InlineAutoData(100, "qwerty", 1.0)]
        [InlineAutoData("xxx", "qwerty", "xxx")]
        [InlineAutoData("xxx", null, "xxx")]
        [Theory]
        public void Convert_HasDescription_DescriptionMatch(object value, object parameter, object expected)
        {
            // Act
            var actual = this.converter.Convert(value, null, parameter, CultureInfo.InvariantCulture);

            Assert.Equal(expected, actual);
        }

        [InlineAutoData(-100, MinusTenThousand)]
        [InlineAutoData("-100", MinusTenThousand)]
        [InlineAutoData(-100.0, MinusTenThousand)]
        [InlineAutoData(100.0, TenThousand)]
        [Theory]
        public void ConvertBack_Execute_DescriptionMatch(object value, object expected)
        {
            // Act
            var actual = this.converter.ConvertBack(value, null, null, CultureInfo.InvariantCulture);

            Assert.Equal(expected, actual);
        }
    }
}
