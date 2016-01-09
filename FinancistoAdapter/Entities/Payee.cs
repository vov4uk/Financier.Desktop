using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[Entity("payee")]
	public class Payee : Entity
	{
		[EntityProperty("title")]
		public string Title { get; set; }
	}
}
