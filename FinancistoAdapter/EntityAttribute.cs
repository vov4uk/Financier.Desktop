using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class EntityAttribute : Attribute
	{
		public string EntityName { get; private set; }
		public EntityAttribute(string entityName)
		{
			if (String.IsNullOrEmpty(entityName)) throw new ArgumentException("Entity name cannot be null or empty.", "key");
			EntityName = entityName;
		}
	}
}
