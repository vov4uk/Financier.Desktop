namespace Financier.Desktop.Tests.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using AutoFixture.Xunit2;
    using Financier.Desktop.Converters;
    using Xunit;

    public class UnixTimeConverterTest
    {
        private readonly IValueConverter converter = new UnixTimeConverter();

        [InlineAutoData(1642152563000, "2022-01-14 13:29:23")]
        [InlineAutoData(1642111200000, "2022-01-14 02:00:00")]
        [Theory]
        public void Convert_GotParameters_ExpectedValues(object value, string expexted)
        {
            string exp;
            DateTime date = DateTime.Parse(expexted).ToUniversalTime();
            exp = date == date.Date ? date.ToString(UnixTimeConverter.FORMAT_DAY) : date.ToString(UnixTimeConverter.FORMAT);

            // Act
            var actual = this.converter.Convert(value, null, null, CultureInfo.InvariantCulture);

            Assert.Equal(exp, actual);
        }

        [InlineAutoData("2022-01-14 11:29:23", 1642152563000)]
        [InlineAutoData("2022-01-14", 1642111200000)]
        [Theory]
        public void ConvertBack_GotParameters_ExpectedValues(string value, long expexted)
        {
            DateTime dateTime = DateTime.Parse(value).ToUniversalTime();

            // Act
            var actual = this.converter.ConvertBack(dateTime, null, null, CultureInfo.InvariantCulture);

            Assert.Equal(expexted, actual);
        }

        [InlineAutoData("xxx")]
        [InlineAutoData(null)]
        [Theory]
        public void Convert_InvalidParameters_ThrowsException(object value)
        {
            // Act
            Assert.Throws<FormatException>(() => this.converter.Convert(value, null, null, CultureInfo.InvariantCulture));
        }
    }
}
