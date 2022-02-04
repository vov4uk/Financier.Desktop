using Financier.Common.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports
{
    public class ReportDynamicDebitCretitPayeeModel : BaseModel
    {
        [DisplayName("Year")]
        [Column("date_year")]
        public long Year { get; protected set; }

        [DisplayName("Month")]
        [Column("date_month")]
        public long Month { get; protected set; }

        public string PeriodDesr => string.Format("{0} {1}", Month, Year);

        [Column("total")]
        [DisplayName("Total")]
        public double? Total { get; protected set; }
    }
}