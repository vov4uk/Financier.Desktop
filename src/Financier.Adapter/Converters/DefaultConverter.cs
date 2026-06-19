using Financier.DataAccess.Data;
using System;
using System.Globalization;

namespace Financier.Adapter.Converters
{
    public class DefaultConverter : IPropertyConverter
    {
        private static readonly NumberFormatInfo Nfi = new NumberFormatInfo { NumberDecimalSeparator = "." };
        private Type _propertyType;
        private Type _resolvedType;

        public Type PropertyType
        {
            get => _propertyType;
            set
            {
                _propertyType = value;
                _resolvedType = Nullable.GetUnderlyingType(value) ?? value;
            }
        }

        public object Convert(object value)
        {
            Type type = _resolvedType;

            if (type == typeof(bool) && value is string s0)
            {
                if (bool.TryParse(s0, out bool result))
                    return result;
                if (int.TryParse(s0, out int i))
                    return System.Convert.ToBoolean(i);
            }
            if (type == typeof(double) && value is string s)
            {
                bool isNum = double.TryParse(s, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var retNum);
                if (!isNum)
                    return default(double?)!;
                return retNum;
            }
            if (type == typeof(float) && value is string s1)
            {
                bool isNum = float.TryParse(s1, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var retNum);
                if (!isNum)
                    return default(float?)!;
                return retNum;
            }
            return System.Convert.ChangeType(value, type);
        }

        public string ConvertBack(object value)
        {
            Type type = _resolvedType;
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
            return System.Convert.ToString(value)!;
        }
    }
}
