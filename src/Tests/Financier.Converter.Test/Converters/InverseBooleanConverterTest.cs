namespace Financier.Converters.Tests
{
    using System.Globalization;
    using AutoFixture.Xunit3;
    using Financier.Converters;
    using Xunit;

    public class InverseBooleanConverterTest
    {
        private readonly InverseBooleanConverter converter = new InverseBooleanConverter();

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
