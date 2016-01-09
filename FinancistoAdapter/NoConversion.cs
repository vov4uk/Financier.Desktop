using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public class NoConversion : IPropertyConverter
	{
		public Type PropertyType { get; set; }

		public object Convert(object value)
		{
			return value;
		}
	}
}
