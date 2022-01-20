using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;

namespace Financier.Reports.Reports
{
    [Header("Структура активов")]
    public class ReportStructureActivesVM : BaseReportVM<ReportStructureActivesModel>
    {
        private const string BaseSqlText = @"
SELECT title,
       Round(
       CASE
              WHEN {0}= 1 THEN total_amount
              ELSE total_amount_indef
       END / 100.00, 2) AS total
FROM   v_account
WHERE  1 = 1 {1}
       /*FILTERS*/";

        public ReportStructureActivesVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override string GetSql()
        {
            string str = string.Empty;
            if (CurentCurrency.ID.HasValue)
            {
                str = string.Format("and currency_id = {0}", CurentCurrency.ID);
            }
            return string.Format(
"\r\n                             select" +
"\r\n                                title," +
"\r\n                                round(case when {0}= 1 then total_amount else total_amount_indef end / 100.00, 2) as total" +
"\r\n                            from v_account" +
"\r\n                            where 1 = 1" +
"\r\n                            {1} /*FILTERS*/" +
"\r\n                    ",
CurentCurrency.ID.HasValue ? 1 : 0,
str);
        }
    }
}