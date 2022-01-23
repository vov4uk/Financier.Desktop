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
SELECT p.title                                                 AS title,
       Round(Sum(tx.from_amount_default_currency) / 100.00, 2) AS total
FROM   (SELECT (SELECT parent._id AS _id
                FROM   category AS node,
                       category AS parent
                WHERE  node.LEFT BETWEEN parent.LEFT AND parent.right
                       AND node._id = t.category_id
                       AND parent._id != t.category_id
                ORDER  BY parent.LEFT ASC
                LIMIT  1) top_parent,
               t.from_amount_default_currency
        FROM   v_report_category_v2 t
        WHERE  ( payee_id > 0 OR category_id > 0 OR project_id > 0 )
        AND t.from_amount < 0
/*FILTERS*/
               {0}
/*FILTERS*/
       ) tx
       INNER JOIN category p
               ON p._id = tx.top_parent
GROUP  BY p._id
ORDER  BY total ASC ";

        public ReportStructureDebitVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {

        }

        protected override string GetSql()
        {
            string str = string.Empty;
            string standartTrnFilter = GetStandartTrnFilter();
            if (standartTrnFilter != string.Empty)
            {
                str = " and " + standartTrnFilter;
            }
            return string.Format(BaseSqlText, str);
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