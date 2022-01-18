namespace Financier.Reports.Common
{
    public class CategoryModel : BaseReportModel
    {
        [Field("_id")]
        public long? ID { get; set; }

        [Field("title")]
        public string title { get; set; }

        public string Title => (title ?? string.Empty).PadLeft((title ?? string.Empty).Length + (int)(level ?? 0L), '-');

        [Field("level")]
        public long? level { get; set; }
    }
}