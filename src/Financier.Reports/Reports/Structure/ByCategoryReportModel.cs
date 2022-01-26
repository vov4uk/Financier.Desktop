using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports.Structure
{
    public class ByCategoryReportModel : BaseReportModel
    {
        [Column("parent_title")]
        [DisplayName("Category")]
        public string Category { get; protected set; }

        [Column("is_expense")]
        public long IsExpense { get; protected set; }

        [Column("parent_id")]
        public long ParentId { get; protected set; }

        [Column("total")]
        [DisplayName("Total")]
        public double Total { get; protected set; }
    }
}
