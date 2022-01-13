using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using Financier.DataAccess.Data;

namespace Financier.DataAccess.View
{
    [DebuggerDisplay("{name} - {from_amount}")]
    public class ByCategoryReport : ByCategoryReportBase, IIdentity
    {
        [Column(IdColumn)]
        public int Id { get; set; } = -1;
    }
}