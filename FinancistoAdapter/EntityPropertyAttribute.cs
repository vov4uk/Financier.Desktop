using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class EntityPropertyAttribute : Attribute
	{
		public string Key { get; private set; }
		public EntityPropertyAttribute(string key)
		{
			if (String.IsNullOrEmpty(key)) throw new ArgumentException("Key cannot be null or empty.", "key");
			Key = key;
		}
	}
}
