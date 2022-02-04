using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Common.Model
{
    public class Years : BaseModel
    {
        [Column("year")]
        public long? Year { get; set; }
    }
}