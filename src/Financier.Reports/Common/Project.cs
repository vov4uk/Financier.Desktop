namespace fcrd
{
    public class Project : BaseReportM
    {
        [Field("_id")]
        public long? ID { get; set; }

        [Field("title")]
        public string Title { get; set; }
    }
}