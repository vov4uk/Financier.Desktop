using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports
{
    public class ReportStructureActivesModel : BaseReportModel
    {
        [Column("title")]
        [DisplayName("Актив")]
        public string Name { get; protected set; }

        [Column("total")]
        [DisplayName("Сумма")]
        public double? Total { get; protected set; }
    }
}