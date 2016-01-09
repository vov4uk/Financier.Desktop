using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[Entity("transactions")]
	public class Transaction : Entity
	{
		[EntityProperty("from_account_id", ForeignKey = typeof(Account))]
		public Account From { get; set; }
		[EntityProperty("to_account_id", ForeignKey = typeof(Account))]
		public Account To { get; set; }
		[EntityProperty("category_id", ForeignKey = typeof(Category))]
		public Category Category { get; set; }
		[EntityProperty("note")]
		public string Note { get; set; }
		[EntityProperty("datetime", Converter = typeof(DateTimeConverter))]
		public DateTime? DateTime { get; set; }
		[EntityProperty("from_amount", Converter = typeof (AmountConverter))]
		public double? FromAmount { get; set; }
		[EntityProperty("to_amount", Converter = typeof(AmountConverter))]
		public double? ToAmount { get; set; }
		[EntityProperty("payee", ForeignKey = typeof(Payee))]
		public Payee Payee { get; set; }
	}
}
