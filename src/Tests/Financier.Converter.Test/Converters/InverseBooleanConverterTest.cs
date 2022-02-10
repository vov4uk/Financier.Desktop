namespace Financier.Converters.Tests
{
    using System.Globalization;
    using System.Windows.Data;
    using AutoFixture.Xunit2;
    using Financier.Converters;
    using Xunit;

    public class InverseBooleanConverterTest
    {
        private readonly IValueConverter converter = new InverseBooleanConverter();

        [InlineAutoData(true, false)]
        [InlineAutoData(false, true)]
        [Theory]
        public void Convert_ValidParameters_OpositeValue(object value, bool expexted)
        {
            // Act
            var actual = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.Equal(expexted, actual);
        }
    }
}