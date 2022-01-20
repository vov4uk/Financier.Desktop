using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Common
{
    public class YearMonths : BaseReportModel
    {
        [Column("year")]
        public long? Year { get; set; }

        [Column("month")]
        public long? Month { get; set; }

        public string Name => string.Format("{0} {1}", Month, Year);
    }
}