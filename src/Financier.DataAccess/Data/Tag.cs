using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.TAG_TABLE)]
    public class Tag : TagBase
    {
        [Column(Backup.SortOrderColumn)]
        public int SortOrder { get; set; }
    }
}
