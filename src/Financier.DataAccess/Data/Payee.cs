using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.PAYEE_TABLE)]
    public class Payee : Entity, IActive
    {
        [Column(IdColumn)]
        public int Id { get; set; } = -1;

        [Column(IsActiveColumn)]
        public bool IsActive { get; set; } = true;

        [Column(TitleColumn)]
        public string Title { get; set; }

        [Column("last_category_id")]
        public long LastCategoryId { get; set; }

        [Column(UpdatedOnColumn)]
        public long UpdatedOn { get; set; }
    }
}