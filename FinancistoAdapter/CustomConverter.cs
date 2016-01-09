using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public abstract class CustomConverter : IPropertyConverter
	{
		public Type PropertyType { get; set; }

		protected abstract object PerformConvertion(string value);

		public object Convert(object value)
		{
			string s = value as string;
			if (s == null) throw new NotSupportedException("Only string values are supported by this convertor.");
			return PerformConvertion(s);
		}
	}
}
