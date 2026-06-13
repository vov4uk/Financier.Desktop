using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Financier.Desktop.Wizards
{
    public class DoubleConvert : DoubleConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            bool isNum = double.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out double retNum);
            if (!isNum)
            {
                return default(double?);
            }
            return retNum;
        }
    }
}
