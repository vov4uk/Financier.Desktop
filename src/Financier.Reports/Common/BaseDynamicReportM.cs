using System.Dynamic;

namespace Financier.Reports.Common
{
    internal class BaseDynamicReportM : BaseReportM
    {
        private ExpandoObject ReportData { get; set; }

        public BaseDynamicReportM() => ReportData = new ExpandoObject();

        public virtual void InitReportData()
        {
        }
    }
}