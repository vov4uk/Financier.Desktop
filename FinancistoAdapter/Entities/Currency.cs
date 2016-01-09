using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[DebuggerDisplay("{Title}")]
	[Entity("currency")]
	public class Currency : Entity
	{
		[EntityProperty("name")]
		public string Name { get; set; }
		[EntityProperty("title")]
		public string Title { get; set; }
	}
}
