using Financier.Reports.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Reports.Reports
{
    public class ReportDynamicRestModel : BaseReportModel
    {
        [Column("year")]
        public long Year { get; protected set; }

        [Column("month")]
        public long Month { get; protected set; }

        [Column("day")]
        public long Day { get; protected set; }

        [DisplayName("Дата")]
        public string Title => $"{Year}.{Month:00}.{Day:00}";

        [DisplayName("Всего в домашней валюте")]
        [Column("total")]
        public double? Total { get; protected set; }
    }
}