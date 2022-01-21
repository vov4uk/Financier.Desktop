using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports
{
    public class ReportDynamicDebitCretitPayeeModel : BaseReportModel
    {
        [DisplayName("Год")]
        [Column("date_year")]
        public long Year { get; protected set; }

        [DisplayName("Месяц")]
        [Column("date_month")]
        public long Month { get; protected set; }

        public string PeriodDesr => string.Format("{0} {1}", Month, Year);

        [Column("total")]
        [DisplayName("Сумма")]
        public double? Total { get; protected set; }
    }
}