using Financier.Common.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports
{
    public class ReportByPeriodMonthCrcModel : BaseModel
    {
        [DisplayName("year")]
        [Column("date_year")]
        public long Year { get; protected set; }

        [Column("date_month")]
        [DisplayName("month")]
        public long Month { get; protected set; }

        public string PeriodDesr => string.Format("{0} {1}", Month, Year);

        [Column("credit_sum")]
        [DisplayName("income")]
        public double? CreditSum { get; protected set; }

        [DisplayName("expense")]
        [Column("debit_sum")]
        public double? DebitSum { get; protected set; }

        [Column("saldo")]
        [DisplayName("saldo")]
        public double? Saldo { get; protected set; }
    }
}
