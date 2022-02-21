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
        [DisplayName("Category")]
        public string Name { get; protected set; }

        [Column("total")]
        [DisplayName("Total")]
        public double? Total { get; protected set; }

        public string Label => $"{Name} ({Total})";
    }
}