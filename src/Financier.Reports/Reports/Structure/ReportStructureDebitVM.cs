using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Reports.Reports
{
    [Header("Income-Expense structure")]
    public class ReportStructureDebitVM : BaseReportVM<ReportStructureDebitModel>
    {
        private bool isIncome;
        public bool IsIncome {

            get => isIncome;
            set
            {
                isIncome = value;
                RaisePropertyChanged(nameof(IsIncome));
            }
        }

        private const string BaseSqlText = @"
SELECT p.title                                            AS title,
       Round(Sum(tx.from_amount_default_crr) / 100.00, 2) AS total
FROM   (SELECT (SELECT parent._id AS _id
                FROM   category AS node,
                       category AS parent
                WHERE  node.LEFT BETWEEN parent.LEFT AND parent.right
                       AND node._id = t.category_id
                ORDER  BY parent.LEFT ASC
                LIMIT  1) top_parent,
               t.from_amount_default_crr
        FROM   v_report_transactions t
        WHERE  category_id > 0 AND from_account_is_include_into_totals = 1
        AND t.from_amount {0} 0
/*FILTERS*/
               {1}
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

            string sign = IsIncome ? ">" : "<";

            return string.Format(BaseSqlText, sign, str);
        }

        protected override PlotModel GetPlotModel(List<ReportStructureDebitModel> list)
        {
            var model = new PlotModel();
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
            return model;
        }
    }
}