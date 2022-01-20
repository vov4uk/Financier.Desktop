using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Common
{
    public class PayeeModel : BaseReportModel
    {
        [Column("_id")]
        public long? ID { get; set; }

        [Column("title")]
        public string Title { get; set; }
    }
}