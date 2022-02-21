using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class LocationModel : TagModel
    {
        [Column("resolved_address")]
        public string Address { get; set; }
    }
}
