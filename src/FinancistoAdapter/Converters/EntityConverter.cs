using System;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter.Converters
{
    public class EntityConverter : IPropertyConverter
    {
        public Type PropertyType { get; set; }

        public object Convert(object value)
        {
            if (value is Entity) 
                return value;
            return null;
        }

        public string ConvertBack(object value)
        {
            if (value is IIdentity entity)
                return entity.Id.ToString();
            return System.Convert.ToString(value);
        }
    }
}
