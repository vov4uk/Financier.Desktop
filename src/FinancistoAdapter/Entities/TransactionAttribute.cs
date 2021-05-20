using System.Diagnostics;

namespace FinancistoAdapter.Entities
{
    [DebuggerDisplay("{Attribute.Title} = {Value} (TranId = {Transaction.Id})")]
    [Entity(Backup.TRANSACTION_ATTRIBUTE_TABLE)]
    public class TransactionAttribute : Entity
    {
        [EntityProperty("transaction_id")]
        public int Transaction { get; set; }

        [EntityProperty("attribute_id")]
        public int Attribute { get; set; }

        [EntityProperty("value")]
        public string Value { get; set; }
    }
}
