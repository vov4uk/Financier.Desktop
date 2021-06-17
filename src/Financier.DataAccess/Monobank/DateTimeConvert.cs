using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Globalization;

namespace Financier.DataAccess.Monobank
{
    public class DateTimeConvert : DefaultTypeConverter
    {
        private const string DATE_TIME_FORMAT = "dd.MM.yyyy HH:mm:ss";
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            DateTime.TryParseExact(text, DATE_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt);
            return dt;
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((DateTime)value).ToString(DATE_TIME_FORMAT);
        }
    }
}
