using Financier.Reports.Common;
using System.ComponentModel;

namespace Financier.Reports.Reports
{
    public class ReportDynamicDebitCretitPayeeM : BaseReportM
    {
        [DisplayName("Год")]
        [Field("date_year")]
        public long Year { get; protected set; }

        [DisplayName("Месяц")]
        [Field("date_month")]
        public long Month { get; protected set; }

        public string PeriodDesr => string.Format("{0} {1}", Month, Year);

        [Field("total")]
        [DisplayName("Сумма")]
        public double? Total { get; protected set; }
    }
}