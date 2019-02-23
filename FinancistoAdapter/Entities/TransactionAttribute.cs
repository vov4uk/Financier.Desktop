using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancistoAdapter.Entities
{
	[DebuggerDisplay("{Attribute.Title} = {Value} (TranId = {Transaction.Id})")]
	[Entity("transaction_attribute")]
	public class TransactionAttribute : Entity
	{
		[EntityProperty("transaction_id")]
		public Transaction Transaction { get; set; }

		[EntityProperty("attribute_id")]
		public AttributeDefinition Attribute { get; set; }

		[EntityProperty("value")]
		public string Value { get; set; }
	}
}
