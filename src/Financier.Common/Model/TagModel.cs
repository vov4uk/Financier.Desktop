using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class TagModel : BaseModel, IActive
    {
        [Column("_id")]
        public int? Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}