using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class TagModel : TagBaseModel
    {
        [Column("sort_order")]
        public int SortOrder { get; set; }
    }
}
