using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.DataAccess.Data
{
    [Keyless]
    [Table(Backup.CATEGORY_ATTRIBUTE_TABLE)]
    public class CategoryAttribute : Entity
    {
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("attribute_id")]
        public int AttributeId { get; set; }
    }
}