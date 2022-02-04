namespace Financier.Converters.Tests
{
    using System.Globalization;
    using System.Windows.Data;
    using AutoFixture.Xunit2;
    using Financier.Common.Entities;
    using Financier.Converters;
    using Xunit;

    public class EnumDescriptionConverterTest
    {
        private readonly IValueConverter converter = new EnumDescriptionConverter();

        [InlineAutoData(PeriodType.Today, "Today")]
        [InlineAutoData(PeriodType.Yesterday, "Yesterday")]
        [InlineAutoData(PeriodType.CurrentWeek, "Current week")]
        [InlineAutoData(PeriodType.PreviousWeek, "Previous week")]
        [InlineAutoData(PeriodType.PreviousAndCurrentWeek, "Previous and current week")]
        [InlineAutoData(PeriodType.CurrentMonth, "Current month")]
        [InlineAutoData(PeriodType.PreviousMonth, "Previous month")]
        [InlineAutoData(PeriodType.PreviousAndCurrentMonth, "Previous and current month")]
        [InlineAutoData(PeriodType.Custom, "Custom")]
        [InlineAutoData(TimeSpanStyles.None, "None")]
        [InlineAutoData(TimeSpanStyles.AssumeNegative, "AssumeNegative")]
        [InlineAutoData("AssumeNegative", "AssumeNegative")]
        [Theory]
        public void Convert_HasDescription_DescriptionMatch(object val, string desc)
        {
            // Act
            var actual = converter.Convert(val, typeof(string), null, CultureInfo.InvariantCulture);

            Assert.Equal(desc, actual);
        }

        [InlineAutoData(PeriodType.Custom)]
        [InlineAutoData(TimeSpanStyles.None)]
        [Theory]
        public void ConvertBack_Execute_EmptyString(object val)
        {
            // Act
            var actual = converter.ConvertBack(val, typeof(string), null, CultureInfo.InvariantCulture);

            Assert.Equal(string.Empty, actual);
        }
    }
}
