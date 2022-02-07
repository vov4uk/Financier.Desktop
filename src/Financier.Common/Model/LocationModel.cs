using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class LocationModel : TagModel
    {
        [Column("resolved_address")]
        public string Address { get; set; }
    }
}
