using Financier.Reports.Common;

namespace Financier.Reports.Reports
{
    [Header("Динамика остатков")]
    public class ReportDynamicRestVM : BaseReportVM<ReportDynamicRestM>
    {
        private const string BaseSqlText = @"
SELECT cr.year  AS year,
       cr.month AS month,
       cr.day   AS day,
       Round(
               (
               SELECT Sum(from_amount_default_crr)
               FROM   transactions trn
               WHERE  Date(trn.datetime) <= cr.date
               AND    to_account_id = 0 ) / 100.00, 2 )AS total
FROM   (
                       SELECT DISTINCT Date(datetime) AS date,
                                       date_year      AS year,
                                       date_month     AS month,
                                       date_day       AS day
                       FROM            transactions
                       WHERE           1 = 1 {0}
                                       /* FILTER */
       ) cr";

        protected override string GetSql()
        {
            string standartTrnFilter = GetStandartTrnFilter();
            return string.Format(
"\r\n                           select" +
"\r\n                                    cr.year as year, cr.month as month, cr.day as day," +
"\r\n                                    round( (select sum(from_amount_default_crr) " +
"\r\n                                     from transactions trn where date(trn.datetime) <= cr.date " +
"\r\n                                            and to_account_id = 0 ) / 100.00, 2 )as total" +
"\r\n                                from " +
"\r\n                                    (select distinct date(datetime) as date, date_year as year, date_month as month, date_day as day" +
"\r\n                                    from transactions" +
"\r\n                                    where 1 = 1 " +
"\r\n                                    {0}/* FILTER */" +
"\r\n                                    ) cr" +
"\r\n                    ",
!string.IsNullOrEmpty(standartTrnFilter) ? " and " + standartTrnFilter : string.Empty);
        }
    }
}