using Financier.DataAccess.Data;
using System;
using System.Globalization;

namespace Financier.Adapter.Converters
{
    public class DefaultConverter : IPropertyConverter
    {
        private static readonly NumberFormatInfo Nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };

        public object Convert(object value)
        {
            Type type = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;

            if (type == typeof(bool) && value is string)
            {
                if (bool.TryParse((string)value, out bool result))
                    return result;
                if (int.TryParse((string)value, out int i))
                    return System.Convert.ToBoolean(i);
            }
            if (type == typeof(double) && value is string s)
            {

                bool isNum = double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var retNum);
                if (!isNum)
                {
                    return default(double?);
                }
                return retNum;
            }
            if (type == typeof(float) && value is string s1)
            {

                bool isNum = float.TryParse(s1, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var retNum);
                if (!isNum)
                {
                    return default(float?);
                }
                return retNum;
            }
            return System.Convert.ChangeType(value, type);
        }

        public string ConvertBack(object value)
        {
            Type type = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;
            if (type == typeof(bool))
            {
                return System.Convert.ToInt32(value).ToString();
            }
            if (type == typeof(IIdentity) || type.BaseType == typeof(IIdentity))
            {
                var entity = value as IIdentity;
                return (entity?.Id ?? 0).ToString();
            }
            if (type == typeof(double))
            {
                return ((double)value).ToString("0.####", Nfi);
            }
            if (type == typeof(float))
            {
                return ((float)value).ToString("0.####", Nfi);
            }
            return System.Convert.ToString(value);
        }

        public Type PropertyType { get; set; }
    }
}
