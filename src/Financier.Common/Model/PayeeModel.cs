using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class PayeeModel : BaseModel
    {
        [Column("_id")]
        public long? Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("is_active")]
        public long IsActive { get; set; }
    }
}