using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[DebuggerDisplay("{Title}")]
	[Entity("category")]
	public class Category : Entity
	{
		private class SplitCategory : Category
		{
			public override int Id
			{
				get { return -1; } 
				set { }
			}

			public override string Title 
			{ 
				get { return "Split"; }
				set { } 
			}
		}

		private static readonly Category _split = new SplitCategory();
		public static Category Split
		{
			get { return _split; }
		}

		[EntityProperty("title")]
		public virtual string Title { get; set; }
	}
}
