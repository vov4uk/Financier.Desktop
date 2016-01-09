using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public class AmountConverter : CustomConverter
	{
		protected override object PerformConvertion(string value)
		{
			double d = double.Parse(value);
			return d / 100;
		}
	}
}
