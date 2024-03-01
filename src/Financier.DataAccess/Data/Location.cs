using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.Data
{
    [DebuggerDisplay("{Title}")]
    [Table(Backup.LOCATIONS_TABLE)]
    public class Location : Tag
    {

        [Column("datetime")]
        public long Date { get; set; }

        [Column("provider")]
        public string Provider { get; set; }

        [Column("accuracy")]
        public string Accuracy { get; set; }

        [Column("latitude")]
        public string Latitude { get; set; }

        [Column("longitude")]
        public string Longitude { get; set; }

        [Column("resolved_address")]
        public string Address { get; set; }

        [Column("count")]
        public int Count { get; set; }
        
        [Column("sort_order")]
        public int SortOrder { get; set; }

    }
}