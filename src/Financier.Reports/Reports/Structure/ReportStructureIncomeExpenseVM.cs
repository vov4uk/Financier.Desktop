using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Reports.Reports
{
    [Header("Income-Expense structure")]
    public class ReportStructureIncomeExpenseVM : BaseReportVM<ReportStructureIncomeExpenseModel>
    {
        private bool isIncome;

        public bool IsIncome
        {
            get => isIncome;
            set
            {
                isIncome = value;
                RaisePropertyChanged(nameof(IsIncome));
            }
        }

        private PlotModel barChartModel;

        public PlotModel BarChartModel
        {
            get => barChartModel;
            private set
            {
                barChartModel = value;
                RaisePropertyChanged(nameof(BarChartModel));
            }
        }

        private const string BaseSqlText = @" /* ReportStructureDebitVM */
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

        public ReportStructureIncomeExpenseVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
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

        protected override PlotModel GetPlotModel(List<ReportStructureIncomeExpenseModel> list)
        {
            BarChartModel = GetBarChartModel(list);
            return GetPieChartModel(list);
        }

        private static PlotModel GetPieChartModel(List<ReportStructureIncomeExpenseModel> list)
        {
            var model = new PlotModel();
            var ps = new PieSeries
            {
                StrokeThickness = 0.0,
                InsideLabelFormat = "",
                OutsideLabelFormat = "{1}: {2:0.00}%",
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

        protected PlotModel GetBarChartModel(List<ReportStructureIncomeExpenseModel> list)
        {
            var plotModel1 = new PlotModel
            {
            };
            var categoryAxis1 = new CategoryAxis
            {
                MinorStep = 1,
                Position = AxisPosition.Left,
                Title = "Category",
                TitleFormatString = "{0}",
            };

            plotModel1.Axes.Add(categoryAxis1);

            var linearAxis1 = new LinearAxis
            {
                AbsoluteMinimum = 0,
                MaximumPadding = 0.06,
                MinimumPadding = 0,
                Position = AxisPosition.Bottom
            };

            plotModel1.Axes.Add(linearAxis1);

            var barSeries1 = new BarSeries
            {
                BaseValue = 0.1,
                StrokeThickness = 1,
                LabelPlacement = LabelPlacement.Base,
                LabelFormatString = IsIncome ? "{0}" : "-{0}",
                FillColor = IsIncome ? OxyColors.Green : OxyColors.Orange,
            };


            foreach (var item in list.OrderBy(x => Math.Abs(x.Total ?? 0)))
            {
                categoryAxis1.ActualLabels.Add(item.Name);
                barSeries1.ActualItems.Add(new BarItem(Math.Abs(item.Total ?? 0), -1));
            }


            plotModel1.Series.Add(barSeries1);

            var legend1 = new Legend
            {
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter
            };
            plotModel1.Legends.Add(legend1);
            return plotModel1;
        }
    }
}