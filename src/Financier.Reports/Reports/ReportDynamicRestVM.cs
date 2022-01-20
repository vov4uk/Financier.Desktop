using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;

namespace Financier.Reports.Reports
{
    [Header("Динамика остатков")]
    public class ReportDynamicRestVM : BaseReportVM<ReportDynamicRestModel>
    {
        private const string BaseSqlText = @"
SELECT cr.year  AS year,
       cr.month AS month,
       cr.day   AS day,
       Round(
               (
               SELECT Sum(from_amount_default_crr)
               FROM   v_report_transactions trn
               WHERE  Date(trn.datetime) <= cr.date
               AND    to_account_id = 0 ) / 100.00, 2 )AS total
FROM   (
                       SELECT DISTINCT Date(datetime) AS date,
                                       date_year      AS year,
                                       date_month     AS month,
                                       date_day       AS day
                       FROM            v_report_transactions
                       WHERE           1 = 1 {0}
                                       /* FILTER */
       ) cr";

        public ReportDynamicRestVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override string GetSql()
        {
            string standartTrnFilter = GetStandartTrnFilter();
            return string.Format(
                                     "select " +
                                        "cr.year as year, " +
                                        "cr.month as month, " +
                                        "cr.day as day, " +
                                        "round( (select sum(from_amount_default_crr) " +
                                        " from v_report_transactions trn where Date(trn.datetime / 1000, 'unixepoch') <= cr.date " +
                                                "and to_account_id = 0 ) / 100.00, 2 ) as total " +
                                    "from " +
                                        "(" +
                                         "select distinct Date(datetime / 1000, 'unixepoch') as date, " +
                                                "date_year as year, " +
                                                "date_month as month, " +
                                                "date_day as day " +
                                         "from v_report_transactions " +
                                         "where 1 = 1 " +
                                         "{0} /* FILTER */ " +
                                      ") cr ",
!string.IsNullOrEmpty(standartTrnFilter) ? " and " + standartTrnFilter : string.Empty);
        }
    }
}