using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.ATTRIBUTES_TABLE)]
    public class AttributeDefinition : Entity, IIdentity
    {

        public const int TYPE_TEXT = 1;
        public const int TYPE_NUMBER = 2;
        public const int TYPE_LIST = 3;
        public const int TYPE_CHECKBOX = 4;

        [Column(IdColumn)]
        public int Id { get; set; } = -1;

        [Column("type")]
        public int Type { get; set; }

        [Column(IsActiveColumn)]
        public bool IsActive { get; set; } = true;

        [Column(TitleColumn)]
        public string Title { get; set; }

        [Column("list_values")]
        public string ListValues { get; set; }

        [Column("default_value")]
        public string DefaultValue { get; set; }

        [NotMapped]
        public long UpdatedOn { get; set; }
    }
}
