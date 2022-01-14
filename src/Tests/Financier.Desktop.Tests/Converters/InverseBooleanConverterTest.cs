namespace Financier.Desktop.Tests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using AutoFixture.Xunit2;
    using Financier.Desktop.Converters;
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
            var actual = this.converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.Equal(expexted, actual);
        }

        [Fact]
        public void ConvertBack_InvalidTargetType_ThorwsException()
        {
            // Act
            Assert.Throws<InvalidOperationException>(() => this.converter.Convert(null, typeof(int), null, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void ConvertBack_NotSupported_ThorwsException()
        {
            // Act
            Assert.Throws<NotSupportedException>(() => this.converter.ConvertBack(null, null, null, CultureInfo.InvariantCulture));
        }
    }
}