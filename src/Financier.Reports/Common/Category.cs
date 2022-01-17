namespace fcrd
{
    public class Category : BaseReportM
    {
        [Field("_id")]
        public long? ID { get; set; }

        [Field("title")]
        public string title { get; set; }

        public string Title => (this.title ?? string.Empty).PadLeft((this.title ?? string.Empty).Length + (int)(this.level ?? 0L), '-');

        [Field("level")]
        public long? level { get; set; }
    }
}