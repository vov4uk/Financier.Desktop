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
        private const string DATE_TIME_FORMAT = "dd.MM.yyyy HH:mm:ss";
        private const string DATE_TIME_FORMAT1 = "yyyy-MM-dd HH:mm:ss";
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (DateTime.TryParseExact(text, DATE_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt))
            {
                return dt;
            }
            DateTime.TryParseExact(text, DATE_TIME_FORMAT1, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var date);
            return date;
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((DateTime)value).ToString(DATE_TIME_FORMAT);
        }
    }
}
