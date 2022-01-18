namespace Financier.Reports.Common
{
    public class ProjectModel : BaseReportModel
    {
        [Field("_id")]
        public long? ID { get; set; }

        [Field("title")]
        public string Title { get; set; }
    }
}