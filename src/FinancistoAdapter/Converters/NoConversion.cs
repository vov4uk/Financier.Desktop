using FinancistoAdapter.Entities;
using System;

namespace FinancistoAdapter.Converters
{
    public class NoConversion : IPropertyConverter
    {
        public Type PropertyType { get; set; }

        public object Convert(object value)
        {
            return value;
        }

        public string ConvertBack(object value)
        {
            if (value is IIdentity entity)
                return entity.Id.ToString();
            return System.Convert.ToString(value);
        }
    }
}
