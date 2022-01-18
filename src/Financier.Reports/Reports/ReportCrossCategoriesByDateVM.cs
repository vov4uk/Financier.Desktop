using Financier.Reports.Common;
using System.Windows;

namespace Financier.Reports.Reports
{
    [Header("По категория за период")]
    internal class ReportCrossCategoriesByDateVM : BaseReportVM<ReportCrossCategoriesByDateModel>
    {
        private const string BaseSqlText = @"
SELECT tx.date_year                           AS date_year,
       tx.date_month                          AS date_month,
       Round(tx.credit_sum, 2)                AS credit_sum,
       Round(tx.debit_sum, 2)                 AS debit_sum,
       Round(tx.credit_sum - tx.debit_sum, 2) AS saldo
FROM   (
                SELECT   date_year,
                         date_month,
                         Sum(
                         CASE
                                  WHEN from_amount > 0 THEN (
                                           CASE
                                                    WHEN {0} = 1 THEN from_amount
                                                    ELSE from_amount_default_crr
                                           END )
                                  ELSE 0
                         END) / 100.00 AS credit_sum,
                         Sum(
                         CASE
                                  WHEN from_amount < 0 THEN - (
                                           CASE
                                                    WHEN {0} = 1 THEN from_amount
                                                    ELSE from_amount_default_crr
                                           END )
                                  ELSE 0
                         END) / 100.00 AS debit_sum
                FROM     transactions
                WHERE    to_account_id = 0
                AND      (
                                  payee_id > 0
                         OR       category_id > 0
                         OR       project_id > 0) {1}
                         /*FILTERS*/
                GROUP BY date_year,
                         date_month
                ORDER BY date_year,
                         date_month ) tx";

        protected override string GetSql()
        {
            string str = string.Empty;
            if (!Payee.ID.HasValue && !Category.ID.HasValue)
            {
                int num = (int)MessageBox.Show("Укажите получателя или категорию!");
                return string.Empty;
            }
            if (CurentCurrency.ID.HasValue)
                str = string.Format("and from_account_crc_id = {0}", CurentCurrency.ID);
            string standartTrnFilter = GetStandartTrnFilter();
            if (standartTrnFilter != string.Empty)
                str = str + " and " + standartTrnFilter;
            return string.Format(
"\r\n                select" +
"\r\n                    tx.date_year as date_year," +
"\r\n                    tx.date_month as date_month," +
"\r\n                    round(tx.credit_sum, 2) as credit_sum," +
"\r\n                    round(tx.debit_sum, 2) as debit_sum," +
"\r\n                    round(tx.credit_sum - tx.debit_sum, 2) as saldo" +
"\r\n                from (" +
"\r\n                select" +
"\r\n                    date_year," +
"\r\n                    date_month," +
"\r\n                    sum( case when from_amount > 0 then (case when {0} = 1 then from_amount else from_amount_default_crr end ) else 0 end) / 100.00 as credit_sum," +
"\r\n                    sum( case when from_amount < 0 then - (case when {0} = 1 then from_amount else from_amount_default_crr end ) else 0 end) / 100.00  as debit_sum" +
"\r\n                from transactions" +
"\r\n                where to_account_id = 0 and (payee_id > 0 or category_id > 0 or project_id > 0)" +
"\r\n                        {1} /*FILTERS*/" +
"\r\n                group by" +
"\r\n                    date_year," +
"\r\n                    date_month" +
"\r\n                order by" +
"\r\n                    date_year," +
"\r\n                    date_month" +
"\r\n                ) tx" +
"\r\n        ", CurentCurrency.ID.HasValue ? 1 : 0, str);
        }
    }
}