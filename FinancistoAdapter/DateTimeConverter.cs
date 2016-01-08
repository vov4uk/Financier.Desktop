using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public class DateTimeConverter : IPropertyConverter
	{
		public object Convert(string value)
		{
			double timestamp = double.Parse(value);
			return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(timestamp);
		}

		public Type PropertyType { get; set; }
	}
}
