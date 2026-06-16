using Financier.Common.Model;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Structure
{
    public class ByCategoryReportModel : BaseModel
    {
        [Column("parent_title")]
        [DisplayName("category")]
        public string Category { get; protected set; }

        [Column("is_expense")]
        public long IsExpense { get; protected set; }

        [Column("parent_id")]
        public long ParentId { get; protected set; }

        [Column("total")]
        [DisplayName("total_label")]
        public double Total { get; protected set; }
    }
}
