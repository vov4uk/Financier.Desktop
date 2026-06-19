namespace Financier.Converters.Tests
{
    using System;
    using System.Globalization;
    using Financier.Common.Entities;
    using Financier.Converters;
    using Xunit;

    public class MccConverterTest
    {
        private readonly MccConverter converter = new MccConverter();

        [InlineData(742, Mcc.veterinary_services)]
        [InlineData(743, Mcc.wine_producers)]
        [Theory]
        public void Convert_KnownMccCode_ReturnsCategoryName(int code, Mcc expectedCategory)
        {
            var actual = converter.Convert(code, null, null, CultureInfo.InvariantCulture);

            Assert.Equal(expectedCategory, actual);
        }

        [Fact]
        public void Convert_UnknownMccCode_ReturnsOriginalValue()
        {
            int unknownCode = -1;

            var actual = converter.Convert(unknownCode, null, null, CultureInfo.InvariantCulture);

            Assert.Equal(unknownCode, actual);
        }

        [Fact]
        public void ConvertBack_Always_ThrowsNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() =>
                converter.ConvertBack("Ветеринарні послуги", null, null, CultureInfo.InvariantCulture));
        }
    }
}
