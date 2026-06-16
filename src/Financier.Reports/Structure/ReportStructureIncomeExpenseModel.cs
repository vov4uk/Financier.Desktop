using Financier.Common.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.Reports
{
    [DebuggerDisplay("{Label}")]
    public class ReportStructureIncomeExpenseModel : BaseModel
    {
        [Column("title")]
        [DisplayName("category")]
        public string Name { get; protected set; }

        [Column("total")]
        [DisplayName("total_label")]
        public double? Total { get; protected set; }

        public string Label => $"{Name} ({Total})";
    }
}
