using System.Dynamic;

namespace Financier.Reports.Common
{
    internal class BaseDynamicReportModel : BaseReportModel
    {
        private ExpandoObject ReportData { get; set; }

        public BaseDynamicReportModel() => ReportData = new ExpandoObject();

        public virtual void InitReportData()
        {
        }
    }
}