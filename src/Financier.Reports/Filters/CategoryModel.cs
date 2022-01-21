using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Common
{
    public class CategoryModel : BaseReportModel
    {
        [Column("_id")]
        public long? ID { get; set; }

        [Column("title")]
        public string title { get; set; }

        public string Title => (title ?? string.Empty).PadLeft((title ?? string.Empty).Length + (int)(level ?? 0L), '-');

        [Column("level")]
        public long? level { get; set; }
    }
}