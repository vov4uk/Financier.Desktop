using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class EntityPropertyAttribute : Attribute
	{
		private Type _converter = typeof (DefaultConverter);

		public string Key { get; private set; }

		public Type Converter
		{
			get { return _converter; }
			set
			{
				if (value != null && !typeof(IPropertyConverter).IsAssignableFrom(value))
					throw new ArgumentException("Converter type must implement IPropertyConverter.", "value");
				_converter = value;
			}
		}

		public EntityPropertyAttribute(string key)
		{
			if (String.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", "key");
			Key = key;
		}
	}
}
