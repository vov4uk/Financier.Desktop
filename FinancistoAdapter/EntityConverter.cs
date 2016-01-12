using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FinancistoAdapter.Entities;

namespace FinancistoAdapter
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
	}
}
