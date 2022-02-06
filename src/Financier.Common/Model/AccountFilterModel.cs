using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class AccountFilterModel : BaseModel
    {
        [Column("_id")]
        public long? Id { get; set; }

        [Column("is_active")]
        public long IsActive { get; set; }

        [Column("sort_order")]
        public long SortOrder { get; set; }

        [Column("title")]
        public string Title { get; set; }
    }
}