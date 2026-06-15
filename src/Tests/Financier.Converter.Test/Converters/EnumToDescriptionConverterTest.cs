namespace Financier.Converters.Tests
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using Financier.Common.Entities;
    using Xunit;

    public class EnumToDescriptionConverterTest
    {
        private readonly EnumToDescriptionConverter converter = new EnumToDescriptionConverter();

        [Fact]
        public void Convert_EnumWithDescription_ReturnsDescription()
        {
            var mccValue = Mcc.veterinary_services;

            var actual = converter.Convert(mccValue, typeof(string), null, CultureInfo.InvariantCulture);

            Assert.IsType<string>(actual);
            Assert.NotNull(actual);
            Assert.NotEmpty((string)actual);
        }

        [Fact]
        public void Convert_NullValue_ReturnsNull()
        {
            var actual = converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

            Assert.Null(actual);
        }

        [Fact]
        public void ConvertBack_DescriptionString_ReturnsEnumValue()
        {
            var mccValue = Mcc.veterinary_services;
            var description = converter.Convert(mccValue, typeof(string), null, CultureInfo.InvariantCulture);

            var actual = converter.ConvertBack(description, typeof(Mcc), null, CultureInfo.InvariantCulture);

            Assert.Equal(mccValue, actual);
        }

        [Fact]
        public void ConvertBack_EnumName_ReturnsEnumValue()
        {
            var actual = converter.ConvertBack("veterinary_services", typeof(Mcc), null, CultureInfo.InvariantCulture);

            Assert.Equal(Mcc.veterinary_services, actual);
        }

        [Fact]
        public void ConvertBack_InvalidValue_ReturnsNull()
        {
            var actual = converter.ConvertBack("invalid_value_that_does_not_exist", typeof(Mcc), null, CultureInfo.InvariantCulture);

            Assert.Null(actual);
        }

        [Fact]
        public void ConvertBack_NonEnumType_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                converter.ConvertBack("some_value", typeof(string), null, CultureInfo.InvariantCulture)
            );
        }
    }
}
