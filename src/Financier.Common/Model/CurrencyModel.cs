using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class CurrencyModel : BaseModel
    {
        [Column("_id")]
        public long? Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }

        [Column("is_default")]
        public long IsDefault { get; set; }
    }
}