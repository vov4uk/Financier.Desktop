using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Financier.Desktop.Wizards
{
    [ExcludeFromCodeCoverage]
    public class DateTimeConvert : DefaultTypeConverter
    {
        private static readonly string[] DATE_TIME_FORMATS =
        {
            "dd.MM.yyyy HH:mm:ss",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd H:mm:ss",
        };

#nullable enable
        public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
        {
            DateTime.TryParseExact(text, DATE_TIME_FORMATS, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt);
            return dt;
        }

        public override string ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((DateTime?)value)?.ToString(DATE_TIME_FORMATS[0]) ?? string.Empty;
        }
#nullable disable
    }
}
