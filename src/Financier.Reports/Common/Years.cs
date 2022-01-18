namespace Financier.Reports.Common
{
    public class Years : BaseReportModel
    {
        [Field("year")]
        public long? Year { get; set; }
    }
}