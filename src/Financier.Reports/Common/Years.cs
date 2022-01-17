namespace Financier.Reports.Common
{
    public class Years : BaseReportM
    {
        [Field("year")]
        public long? Year { get; set; }
    }
}