using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public class DateTimeConverter : IPropertyConverter
	{
		public object Convert(object value)
		{
			string s = value as string;
			if (s == null) throw new NotSupportedException("Only string values are supported for datetime conversion.");
			double timestamp = double.Parse(s);
			return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(timestamp);
		}

		public Type PropertyType { get; set; }
	}
}
