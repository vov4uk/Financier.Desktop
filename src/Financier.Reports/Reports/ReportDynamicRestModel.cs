using Financier.Reports.Common;
using System.ComponentModel;

namespace Financier.Reports.Reports
{
    public class ReportDynamicRestModel : BaseReportModel
    {
        [Field("year")]
        [DisplayName("Год")]
        public long Year { get; protected set; }

        [DisplayName("Месяц")]
        [Field("month")]
        public long Month { get; protected set; }

        [DisplayName("День")]
        [Field("day")]
        public long Day { get; protected set; }

        public string Title => string.Format("{0}.{1}.{2}", Day, Month, Year);

        [DisplayName("Всего в домашней валюте")]
        [Field("total")]
        public double? Total { get; protected set; }
    }
}