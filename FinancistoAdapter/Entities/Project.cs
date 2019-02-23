using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[DebuggerDisplay("{Title}")]
	[Entity("project")]
	public class Project : Entity
	{
		[EntityProperty("title")]
		public string Title { get; set; }
	}
}
