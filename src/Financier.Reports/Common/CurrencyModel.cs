namespace Financier.Reports.Common
{
    public class CurrencyModel : BaseReportModel
    {
        [Field("_id")]
        public long? ID { get; set; }

        [Field("name")]
        public string Name { get; set; }

        [Field("symbol")]
        public string Symbol { get; set; }
    }
}