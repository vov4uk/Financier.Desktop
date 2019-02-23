using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[DebuggerDisplay("{Title}")]
	[Entity("attributes")]
	public class AttributeDefinition : Entity
	{
		[EntityProperty("title")]
		public string Title { get; set; }

		[EntityProperty("default_value")]
		public string DefaultValue { get; set; }
	}
}
