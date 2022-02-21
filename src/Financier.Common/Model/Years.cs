using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class Years : BaseModel
    {
        [Column("year")]
        public int? Year { get; set; }
    }
}