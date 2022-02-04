namespace Financier.Converters.Tests
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using AutoFixture.Xunit2;
    using Financier.Converters;
    using Xunit;

    public class StringEmptyToVisibilityConverterTest
    {
        private readonly IValueConverter converter = new StringEmptyToVisibilityConverter();

        [InlineAutoData("", Visibility.Visible)]
        [InlineAutoData(null, Visibility.Visible)]
        [InlineAutoData(123, Visibility.Visible)]
        [InlineAutoData("xxx", Visibility.Collapsed)]
        [Theory]
        public void Convert_GotParameters_ExpectedValues(object value, Visibility expexted)
        {
            // Act
            var actual = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.Equal(expexted, actual);
        }

        [Fact]
        public void ConvertBack_NotSupported_ReturnNull()
        {
            // Act
            var actual = converter.ConvertBack(null, null, null, CultureInfo.InvariantCulture);

            Assert.Null(actual);
        }
    }
}
