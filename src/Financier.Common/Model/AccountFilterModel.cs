using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class AccountFilterModel : BaseModel, IActive
    {
        [Column("_id")]
        public int? Id { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("sort_order")]
        public int SortOrder { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("currency_name")]
        public string CurrencyName { get; set; }

        [Column("currency_id")]
        public int CurrencyId { get; set; }

        [Column("total_amount")]
        public long TotalAmount { get; set; }
    }
}