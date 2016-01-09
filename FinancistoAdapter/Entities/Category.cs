using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[Entity("category")]
	public class Category : Entity
	{
		[EntityProperty("title")]
		public string Title { get; set; }
	}
}
