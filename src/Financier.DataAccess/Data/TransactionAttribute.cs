using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [Keyless]
    [DebuggerDisplay("{Attribute.Title} = {Value} (TranId = {Transaction.Id})")]
    [Table(Backup.TRANSACTION_ATTRIBUTE_TABLE)]
    public class TransactionAttribute : Entity
    {

        [ForeignKey("Transaction")]
        [Key, Column("transaction_id")]
        public int TransactionId { get; set; }

        [ForeignKey("Attribute")]
        [Key, Column("attribute_id")]
        public int AttributeDefinition { get; set; }

        [Column("value")]
        public string Value { get; set; }

        public virtual Transaction Transaction { get; set;}

        public virtual AttributeDefinition Attribute { get; set;}
    }
}
