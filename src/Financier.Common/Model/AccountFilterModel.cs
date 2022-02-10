using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class AccountFilterModel : BaseModel, IActive
    {
        [Column("_id")]
        public long? Id { get; set; }

        [Column("is_active")]
        public long Is_Active { get; set; }

        public bool IsActive => Is_Active == 1;

        [Column("sort_order")]
        public long SortOrder { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("currency_name")]
        public string CurrencyName { get; set; }

        [Column("currency_id")]
        public long CurrencyId { get; set; }

        [Column("total_amount")]
        public long TotalAmount { get; set; }
    }
}