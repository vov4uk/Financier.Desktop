using Financier.DataAccess.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Financier.DataAccess.View
{
    [DebuggerDisplay("{name} - {from_amount_default_currency}")]
    public class ByCategoryReportV2 : ByCategoryReportBase, IIdentity
    {
        [Column(IdColumn)]
        public int Id { get; set; } = -1;

        public long from_amount_default_currency { get; set; }

        public long? to_amount_default_currency { get; set; }
    }
}