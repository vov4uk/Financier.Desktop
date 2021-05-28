using Financier.DataAccess.Data;
using System;
using System.Globalization;

namespace FinancistoAdapter.Converters
{
    public class DefaultConverter : IPropertyConverter
    {
        private static NumberFormatInfo nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

        public object Convert(object value)
        {
            Type type = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;
            try
            {
                if (type == typeof(bool) && value is string)
                {
                    bool result;
                    if (bool.TryParse((string)value, out result))
                        return result;
                    int i;
                    if (int.TryParse((string)value, out i))
                        return System.Convert.ToBoolean(i);
                }
                if (type == typeof(double) && value is string)
                {

                    bool isNum = double.TryParse((string)value, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out var retNum);
                    if (!isNum)
                    {
                        return default(double?);
                    }
                    return retNum;
                }
                if (type == typeof(float) && value is string)
                {

                    bool isNum = float.TryParse((string)value, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out var retNum);
                    if (!isNum)
                    {
                        return default(float?);
                    }
                    return retNum;
                }
                return System.Convert.ChangeType(value, type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConvertBack(object value)
        {
            Type type = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;
            if (type == typeof(bool))
            {
                return System.Convert.ToInt32(value).ToString();
            };
            if (type == typeof(IIdentity) || type.BaseType == typeof(IIdentity))
            {
                var entity = value as IIdentity;
                return (entity?.Id ?? 0).ToString();
            }
            if (type == typeof(double))
            {
                return ((double)value).ToString("0.####", nfi);
            }
            if (type == typeof(float))
            {
                return ((float)value).ToString("0.####", nfi);
            }
            return System.Convert.ToString(value);
        }

        public Type PropertyType { get; set; }
    }
}
