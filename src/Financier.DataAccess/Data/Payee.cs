using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.PAYEE_TABLE)]
    public class Payee : TagBase
    {
        [Column("last_category_id")]
        public long LastCategoryId { get; set; }
    }
}