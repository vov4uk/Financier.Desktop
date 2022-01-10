using Financier.DataAccess.Data;
using Financier.Adapter.Converters;
using System;
using System.Reflection;

namespace Financier.Adapter
{
    public class EntityPropertyInfo
    {
        private readonly SetValueDelegate _delegate;

        public EntityPropertyInfo(PropertyInfo info)
        {
            PropertyName = info.Name;
            PropertyType = info.PropertyType;
            _delegate = info.SetValue;
        }

        private delegate void SetValueDelegate(object entity, object value);
        public IPropertyConverter Converter { get; set; }
        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public void SetValue(Entity entity, object value)
        {
            object v = Converter.Convert(value);
            _delegate(entity, v);
        }
    }
}