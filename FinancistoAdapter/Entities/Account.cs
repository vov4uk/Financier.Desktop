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
		[EntityProperty("creation_date", Converter = typeof (DateTimeConverter))]
		public DateTime? CreationDate { get; set; }
		[EntityProperty("currency_id", ForeignKey = typeof (Currency))]
		public Currency Currency { get; set; }
		[EntityProperty("total_amount", Converter = typeof (AmountConverter))]
		public double? TotalAmount { get; set; }
		[EntityProperty("type")]
		public string Type { get; set; }
		[EntityProperty("issuer")]
		public string Issuer { get; set; }
		[EntityProperty("is_active")]
		public bool IsActive { get; set; }
		[EntityProperty("is_include_into_totals")]
		public bool IsIncludeIntoTotals { get; set; }
		[EntityProperty("card_issuer")]
		public string CardIssuer { get; set; }
		[EntityProperty("updated_on", Converter = typeof(DateTimeConverter))]
		public DateTime? UpdatedOn { get; set; }
		[EntityProperty("last_transaction_date", Converter = typeof(DateTimeConverter))]
		public DateTime? LastTransactionDate { get; set; }
	}
}
