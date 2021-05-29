using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace Financier.DataAccess.Monobank
{
    public class DoubleConvert : DoubleConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return base.ConvertToString(value, row, memberMapData);
        }

        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            bool isNum = double.TryParse(Convert.ToString(text), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out double retNum);
            if (!isNum)
            {
                return default(double?);
            }
            return base.ConvertFromString(text, row, memberMapData);
        }
    }
}
