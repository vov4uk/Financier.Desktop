using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Reports.Reports
{
    [Header("Структура расходов")]
    public class ReportStructureDebitVM : BaseReportVM<ReportStructureDebitModel>
    {
        private const string BaseSqlText = @"
SELECT     p.title                       AS title,
           - Round(tx.total / 100.00, 2) AS total
FROM       (
                    SELECT   t.category_id,
                             Sum(
                             CASE
                                      WHEN {0}=1 THEN t.from_amount
                                      ELSE t.from_amount_default_crr
                             END) AS total
                    FROM     v_report_transactions t
                    WHERE    (
                                      payee_id > 0
                             OR       category_id > 0
                             OR       project_id > 0)
                    AND      t.from_amount < 0 {1}
                             /*FILTERS*/
                    GROUP BY t.category_id ) tx
INNER JOIN category p
ON         p._id = tx.category_id";

        public ReportStructureDebitVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {

        }

        protected override string GetSql()
        {
            string str = string.Empty;
            if (CurentCurrency.ID.HasValue)
            {
                str = string.Format(" and from_account_crc_id = {0}", CurentCurrency.ID);
            }
            string standartTrnFilter = GetStandartTrnFilter();
            if (standartTrnFilter != string.Empty)
            {
                str = str + " and " + standartTrnFilter;
            }
            return string.Format(
"\r\n                            select" +
"\r\n                                p.title as title," +
"\r\n                                round(tx.total / 100.00, 2) as total" +
"\r\n                            from (" +
"\r\n                            select t.category_id, sum(case when {0}=1 then t.from_amount else t.from_amount_default_crr end) as total" +
"\r\n                            from v_report_transactions t" +
"\r\n                            where " +
"\r\n                                 (payee_id > 0 or category_id > 0 or project_id > 0) and t.from_amount < 0" +
"\r\n                                {1} /*FILTERS*/" +
"\r\n                            group by" +
"\r\n                                t.category_id" +
"\r\n                                ) tx inner join category p on p._id = tx.category_id" +
"\r\n                            order by total asc",
                CurentCurrency.ID.HasValue ? 1 : 0,
                str);
        }

        protected override void SetupSeries(List<ReportStructureDebitModel> list)
        {

            var model = new PlotModel { Title = "Структура расходов"};
            var ps = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0,
            };

            var series = list.Select(x => new PieSlice(x.Label, x.Total ?? 0));

            foreach (var item in series)
            {
                ps.Slices.Add(item);
            }
            model.Series.Add(ps);
            PlotModel = model;
        }
    }
}