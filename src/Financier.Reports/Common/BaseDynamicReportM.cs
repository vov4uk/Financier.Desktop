using System.Dynamic;

namespace fcrd
{
    internal class BaseDynamicReportM : BaseReportM
    {
        private ExpandoObject ReportData { get; set; }

        public BaseDynamicReportM() => this.ReportData = new ExpandoObject();

        public virtual void InitReportData()
        {
        }
    }
}