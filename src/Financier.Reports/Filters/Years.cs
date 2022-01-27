using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Common
{
    public class Years : BaseReportModel
    {
        [Column("year")]
        public long? Year { get; set; }
    }
}