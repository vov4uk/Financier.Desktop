using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public class DefaultConverter : IPropertyConverter
	{
		public object Convert(string value)
		{
			return System.Convert.ChangeType(value, Nullable.GetUnderlyingType(PropertyType) ?? PropertyType);
		}

		public Type PropertyType { get; set; }
	}
}
