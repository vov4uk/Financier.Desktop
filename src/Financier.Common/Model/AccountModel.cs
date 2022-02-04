using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class AccountModel : BaseModel
    {
        [Column("_id")]
        public long? ID { get; set; }

        [Column("title")]
        public string Title { get; set; }
    }
}