using System;
using System.CodeDom;
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
		[EntityProperty("creation_date", Converter = typeof(DateTimeConverter))]
		public DateTime? CreationDate { get; set; }
		[EntityProperty("currency_id", ForeignKey = typeof(Currency))]
		public Currency Currency { get; set; }
		[EntityProperty("total_amount")]
		public long? TotalAmount { get; set; }
		[EntityProperty("type")]
		public string Type { get; set; }
		[EntityProperty("issuer")]
		public string Issuer { get; set; }
		[EntityProperty("is_active")]
		public bool IsActive { get; set; }
	}
}
