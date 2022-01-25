using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.Reports.Reports
{
    [DebuggerDisplay("{Label}")]
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