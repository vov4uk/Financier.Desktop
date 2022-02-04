using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class LocationModel
    {
        [Column("_id")]
        public long? ID { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("is_active")]
        public long IsActive { get; set; }
    }
}
