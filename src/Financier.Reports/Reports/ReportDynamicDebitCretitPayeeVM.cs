using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using System.Windows;

namespace Financier.Reports.Reports
{
    [Header("Динамика расходов-доходов")]
    public class ReportDynamicDebitCretitPayeeVM : BaseReportVM<ReportDynamicDebitCretitPayeeModel>
    {
        private const string BaseSqlText = @"
SELECT   tx.date_year                AS date_year,
         tx.date_month               AS date_month,
         Round(tx.total / 100.00, 2) AS total
FROM     (
                  SELECT   trn.date_year,
                           trn.date_month,
                           trn.category_id,
                           Sum(
                           CASE
                                    WHEN {0} = 1 THEN from_amount
                                    ELSE from_amount_default_crr
                           END) AS total
                  FROM     v_report_transactions trn
                  WHERE    (
                                    payee_id > 0
                           OR       category_id > 0
                           OR       project_id > 0) {1}
                           /*FILTERS*/
                  GROUP BY trn.date_year,
                           trn.date_month ) tx
ORDER BY tx.date_year,
         tx.date_month";

        public ReportDynamicDebitCretitPayeeVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override string GetSql()
        {
            long? id;
            int num1;
            if (!Payee.ID.HasValue)
            {
                id = Category.ID;
                num1 = id.HasValue ? 1 : 0;
            }
            else
                num1 = 1;
            if (num1 == 0)
            {
                MessageBox.Show("Укажите получателя или категорию!");
                return string.Empty;
            }
            string str = string.Empty;
            id = CurentCurrency.ID;
            if (id.HasValue)
            {
                str = string.Format(" and from_account_crc_id = {0}", CurentCurrency.ID);
            }
            string standartTrnFilter = GetStandartTrnFilter();
            if (standartTrnFilter != string.Empty)
            {
                str = str + " and " + standartTrnFilter;
            }
            id = CurentCurrency.ID;
            return string.Format(
"\r\n                        select" +
"\r\n                            tx.date_year as date_year, tx.date_month as date_month, round(tx.total / 100.00, 2) as total" +
"\r\n                        from" +
" \r\n                        (select trn.date_year, trn.date_month, trn.category_id, sum(case when {0} = 1 then from_amount else from_amount_default_crr end) as total" +
"\r\n                        from v_report_transactions trn" +
" \r\n                        where (payee_id > 0 or category_id > 0 or project_id > 0)" +
"\r\n                            {1} /*FILTERS*/" +
"\r\n                        group by trn.date_year, trn.date_month ) tx " +
"\r\n                        order by " +
"\r\n                            tx.date_year, tx.date_month" +
"\r\n        ", id.HasValue ? 1 : 0, str);
        }
    }
}