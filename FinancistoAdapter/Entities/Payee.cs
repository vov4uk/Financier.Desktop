using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[DebuggerDisplay("{Title}")]
	[Entity("payee")]
	public class Payee : Entity
	{
		[EntityProperty("title")]
		public string Title { get; set; }
	}
}
