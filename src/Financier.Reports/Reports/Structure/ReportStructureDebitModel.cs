using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports
{
    public class ReportStructureDebitModel : BaseReportModel
    {
        [Column("title")]
        [DisplayName("Category")]
        public string Name { get; protected set; }

        [Column("total")]
        [DisplayName("Total")]
        public double? Total { get; protected set; }

        public string Label => $"{Name} ({Total})";
    }
}