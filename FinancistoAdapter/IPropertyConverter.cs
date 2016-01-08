using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter
{
	public interface IPropertyConverter
	{
		Type PropertyType { get; set; }
		object Convert(string value);
	}
}
