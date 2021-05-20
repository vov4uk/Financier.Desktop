using FinancistoAdapter.Entities;
using System;

namespace FinancistoAdapter.Converters
{
    public class DefaultConverter : IPropertyConverter
    {
        public object Convert(object value)
        {
            Type type = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;
            if (type == typeof (bool) && value is string)
            {
                bool result;
                if (bool.TryParse((string) value, out result))
                    return result;
                int i;
                if (int.TryParse((string) value, out i))
                    return System.Convert.ToBoolean(i);
            }
            return System.Convert.ChangeType(value, type);
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
            return System.Convert.ToString(value);
        }

        public Type PropertyType { get; set; }
    }
}
