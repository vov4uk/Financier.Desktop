using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class TagModel : BaseModel
    {
        [Column("_id")]
        public long? Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("is_active")]
        public long Is_Active { get; set; }

        public bool IsActive => Is_Active == 1;
    }
}