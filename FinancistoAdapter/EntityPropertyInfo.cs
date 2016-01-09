using System;
using System.Reflection;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
{
	public class EntityPropertyInfo
	{
		private delegate void SetValueDelegate(object entity, object value);

		private SetValueDelegate _delegate;

		public EntityPropertyInfo(PropertyInfo info)
		{
			PropertyName = info.Name;
			PropertyType = info.PropertyType;
			_delegate = info.SetValue;
		}

		public string PropertyName { get; private set; }

		public Type PropertyType { get; private set; }

		public void SetValue(Entity entity, string value)
		{
			object v = Converter.Convert(value);
			_delegate(entity, v);
		}

		public void SetValue(Entity entity, object pureValue)
		{
			_delegate(entity, pureValue);
		}

		public IPropertyConverter Converter { get; set; }
	}
}