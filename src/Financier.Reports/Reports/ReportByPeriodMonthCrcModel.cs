using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports
{
    public class ReportByPeriodMonthCrcModel : BaseReportModel
    {
        [DisplayName("Год")]
        [Column("date_year")]
        public long Year { get; protected set; }

        [Column("date_month")]
        [DisplayName("Месяц")]
        public long Month { get; protected set; }

        public string PeriodDesr => string.Format("{0} {1}", Month, Year);

        [Column("credit_sum")]
        [DisplayName("Приход")]
        public double? CreditSum { get; protected set; }

        [DisplayName("Расход")]
        [Column("debit_sum")]
        public double? DebitSum { get; protected set; }

        [Column("saldo")]
        [DisplayName("Сальдо")]
        public double? Saldo { get; protected set; }
    }
}