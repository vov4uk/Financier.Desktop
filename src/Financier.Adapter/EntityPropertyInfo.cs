using Financier.DataAccess.Data;
using Financier.Adapter.Converters;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Financier.Adapter
{
    public class EntityPropertyInfo
    {
        private readonly Action<Entity, object> _setter;

        public EntityPropertyInfo(PropertyInfo info)
        {
            PropertyName = info.Name;
            PropertyType = info.PropertyType;
            _setter = BuildSetter(info);
        }

        public IPropertyConverter Converter { get; set; }
        public string PropertyName { get; private set; }
        public Type PropertyType { get; private set; }

        public void SetValue(Entity entity, object value)
        {
            _setter(entity, Converter.Convert(value));
        }

        private static Action<Entity, object> BuildSetter(PropertyInfo prop)
        {
            var entityParam = Expression.Parameter(typeof(Entity), "e");
            var valueParam = Expression.Parameter(typeof(object), "v");
            var castEntity = Expression.Convert(entityParam, prop.DeclaringType!);

            Expression typedValue;
            Type underlying = Nullable.GetUnderlyingType(prop.PropertyType);
            if (underlying != null)
            {
                // Handles int? <- boxed int: (int?)(int)value
                typedValue = Expression.Convert(Expression.Convert(valueParam, underlying), prop.PropertyType);
            }
            else
            {
                typedValue = Expression.Convert(valueParam, prop.PropertyType);
            }

            var assign = Expression.Assign(Expression.Property(castEntity, prop), typedValue);
            return Expression.Lambda<Action<Entity, object>>(assign, entityParam, valueParam).Compile();
        }
    }
}
