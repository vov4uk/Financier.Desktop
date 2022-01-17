using Financier.Reports.Common;
using System.ComponentModel;

namespace Financier.Reports.Reports
{
    public class ReportStructureCreditM : BaseReportM
    {
        [Field("title")]
        [DisplayName("Получатель")]
        public string Name { get; protected set; }

        [Field("total")]
        [DisplayName("Сумма")]
        public double? Total { get; protected set; }
    }
}