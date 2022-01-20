using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports
{
    public class ReportDynamicRestModel : BaseReportModel
    {
        [Column("year")]
        [DisplayName("Год")]
        public long Year { get; protected set; }

        [DisplayName("Месяц")]
        [Column("month")]
        public long Month { get; protected set; }

        [DisplayName("День")]
        [Column("day")]
        public long Day { get; protected set; }

        public string Title => string.Format("{0}.{1}.{2}", Day, Month, Year);

        [DisplayName("Всего в домашней валюте")]
        [Column("total")]
        public double? Total { get; protected set; }
    }
}