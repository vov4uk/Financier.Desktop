using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Common
{
    public class CurrencyModel : BaseReportModel
    {
        [Column("_id")]
        public long? ID { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("symbol")]
        public string Symbol { get; set; }
    }
}