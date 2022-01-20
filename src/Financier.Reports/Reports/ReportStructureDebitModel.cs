using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports
{
    public class ReportStructureDebitModel : BaseReportModel
    {
        [Column("title")]
        [DisplayName("Получатель")]
        public string Name { get; protected set; }

        [Column("total")]
        [DisplayName("Сумма")]
        public double? Total { get; protected set; }
    }
}