using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;

namespace Financier.Desktop.Wizards
{
    public class DoubleConvert : DoubleConverter
    {
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