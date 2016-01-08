using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[Entity("account")]
	public class Account : Entity
	{
		[EntityProperty("title")]
		public string Title { get; set; }
		//[EntityProperty("creation_date")]
		public DateTime? CreationDate { get; set; }
		[EntityProperty("currency_id")]
		public int? CurrencyId { get; set; }
		[EntityProperty("total_amount")]
		public long? TotalAmount { get; set; }
	}
}
