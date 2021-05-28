using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Globalization;

namespace Financier.Desktop.MonoWizard.Model
{
    public class DateTimeConvert : DefaultTypeConverter
    {
        private const string DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            DateTime.TryParseExact(text, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt);
            return dt;
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return ((DateTime)value).ToString(DateTimeFormat);
        }
    }
}
