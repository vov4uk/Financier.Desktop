using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	public abstract class Entity
	{
		[EntityProperty("_id")]
		public virtual int Id { get; set; }
	}
}
