using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public class DefaultConverter : IPropertyConverter
	{
		public object Convert(object value)
		{
			Type type = Nullable.GetUnderlyingType(PropertyType) ?? PropertyType;
			if (type == typeof (bool) && value is string)
			{
				bool result;
				if (bool.TryParse((string) value, out result))
					return result;
				int i;
				if (int.TryParse((string) value, out i))
					return System.Convert.ToBoolean(i);
			}
			return System.Convert.ChangeType(value, type);
		}

		public Type PropertyType { get; set; }
	}
}
