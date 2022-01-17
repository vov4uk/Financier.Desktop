namespace Financier.Reports.Common
{
    public class Account : BaseReportM
    {
        [Field("_id")]
        public long? ID { get; set; }

        [Field("title")]
        public string Title { get; set; }
    }
}