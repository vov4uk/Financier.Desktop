using Financier.Common.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports
{
    public class ReportByPeriodMonthCrcModel : BaseModel
    {
        [DisplayName("Year")]
        [Column("date_year")]
        public long Year { get; protected set; }

        [Column("date_month")]
        [DisplayName("Month")]
        public long Month { get; protected set; }

        public string PeriodDesr => string.Format("{0} {1}", Month, Year);

        [Column("credit_sum")]
        [DisplayName("Income")]
        public double? CreditSum { get; protected set; }

        [DisplayName("Expanse")]
        [Column("debit_sum")]
        public double? DebitSum { get; protected set; }

        [Column("saldo")]
        [DisplayName("Saldo")]
        public double? Saldo { get; protected set; }
    }
}