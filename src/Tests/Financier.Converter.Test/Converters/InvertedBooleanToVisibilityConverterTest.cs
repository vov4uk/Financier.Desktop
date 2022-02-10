namespace Financier.Converters.Tests
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using AutoFixture.Xunit2;
    using Financier.Converters;
    using Xunit;

    public class InvertedBooleanToVisibilityConverterTest
    {
        private readonly IValueConverter converter = new InvertedBooleanToVisibilityConverter();

        [InlineAutoData(true, Visibility.Collapsed)]
        [InlineAutoData(false, Visibility.Visible)]
        [Theory]
        public void Convert_ValidParameters_OpositeValue(object value, Visibility expexted)
        {
            // Act
            var actual = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.Equal(expexted, actual);
        }

        [InlineAutoData(Visibility.Collapsed, true)]
        [InlineAutoData(Visibility.Visible, false)]
        [Theory]
        public void ConvertBack_ValidParameters_OpositeValue(object value, bool expexted)
        {
            // Act
            var actual = converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.Equal(expexted, actual);
        }
    }
}