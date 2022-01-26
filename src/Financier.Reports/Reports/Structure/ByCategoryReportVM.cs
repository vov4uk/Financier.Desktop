using Financier.DataAccess.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Financier.Reports.Common;
using OxyPlot;
using Financier.Reports.Converters;
using Financier.Reports.Reports.Structure;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Linq;
using OxyPlot.Legends;

namespace Financier.Reports.Reports
{
    [Header("By category")]
    [ExcludeFromCodeCoverage]
    public class ByCategoryReportVM : BaseReportVM<ByCategoryReportModel>
    {
        private const string BaseSqlText = @" /* ByCategoryReportModel */
SELECT parent_id,
       parent_title,
       is_expense,
       Round(Sum(tx.from_amount_default_crr) / 100.00, 2) AS total
FROM   (
        SELECT ctgr.parent_id,
               parent_level,
               parent_title,
               t.category_id,
               parent_left,
               parent_right,
               t.from_amount_default_crr,
               cast(t.from_amount < 0 as boolean) as is_expense
        FROM   v_report_transactions t 
        INNER JOIN (SELECT parent._id AS parent_id,
                           parent.title as parent_title,
                           parent.left as parent_left,
                           parent.right as parent_right,
                           node._id as node_id,
                           node.title as node_title,
                           (select count(*) from category x where x.left < node.left and x.[right] > node.[right] ) as node_level,
                           (select count(*) from category x where x.left < parent.left and x.[right] > parent.[right] ) as parent_level
                FROM   category AS node,
                       category AS parent
                WHERE  node.LEFT BETWEEN parent.LEFT AND parent.right
                ORDER  BY parent.LEFT ASC ) ctgr
                on ctgr.node_id = t.category_id
        WHERE  t.category_id > 0 AND from_account_is_include_into_totals = 1
/*FILTERS*/
               {0} ) tx
WHERE  {1}
/*FILTERS*/
GROUP  BY parent_id,  parent_title , is_expense
ORDER  BY total ASC ";

        private PlotModel pieChartModel;

        public PlotModel PieChartModel
        {
            get => pieChartModel;
            private set
            {
                pieChartModel = value;
                RaisePropertyChanged(nameof(PieChartModel));
            }
        }

        private PeriodType periodType;

        public ByCategoryReportVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
            PeriodType = PeriodType.Today;
            UpdatePeriod(PeriodType);
        }

        public PeriodType PeriodType
        {
            get => periodType;
            set
            {
                if (SetProperty(ref periodType, value))
                {
                    RaisePropertyChanged(nameof(PeriodType));
                    UpdatePeriod(value);
                }
            }
        }

        protected override PlotModel GetPlotModel(List<ByCategoryReportModel> list)
        {
            PieChartModel = GetPieChartModel(list);
            return GetBarChart(list);
        }

        private static PlotModel GetPieChartModel(List<ByCategoryReportModel> list)
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

            var groups = list.GroupBy(x => x.ParentId);
            foreach (var item in groups.OrderBy(x => x.Max(y => Math.Abs(y.Total))))
            {
                ps.Slices.Add(new PieSlice(item.First().Category, Math.Abs(item.Sum(x => x.Total))));
            }

            model.Series.Add(ps);
            return model;
        }

        private static PlotModel GetBarChart(List<ByCategoryReportModel> list)
        {
            var plotModel1 = new PlotModel();
            var categoryAxis1 = new CategoryAxis
            {
                MinorStep = 1,
                Position = AxisPosition.Left,
                Title = "Category",
                TitleFormatString = "{0}",
            };

            var linearAxis1 = new LinearAxis
            {
                MinimumPadding = 0,
                Position = AxisPosition.Bottom
            };

            var incomeSeries = new BarSeries
            {
                Title = "Income",
                LabelFormatString = "{0}",
                LabelPlacement = LabelPlacement.Base
            };
            var ExpenseSeries = new BarSeries
            {
                Title = "Expense",
                LabelFormatString = "-{0}",
                LabelPlacement = LabelPlacement.Base
            };

            int i = 0;
            List<string> names = new List<string>();
            var groups = list.GroupBy(x => x.ParentId);
            foreach (var item in groups.OrderBy(x => x.Max(y => Math.Abs(y.Total))))
            {
                names.Add(item.First().Category);
                foreach (var cat in item)
                {
                    if (cat.IsExpense == 0)
                    {
                        incomeSeries.Items.Add(new BarItem(Math.Abs(cat.Total), i));
                    }
                    else
                    {
                        ExpenseSeries.Items.Add(new BarItem(Math.Abs(cat.Total), i));
                    }
                }
                i++;
            }

            var legend1 = new Legend();
            legend1.LegendPosition = LegendPosition.RightBottom;

            plotModel1.Legends.Add(legend1);

            categoryAxis1.ItemsSource = names;
            plotModel1.Series.Add(incomeSeries);
            plotModel1.Axes.Add(linearAxis1);
            plotModel1.Axes.Add(categoryAxis1);
            plotModel1.Series.Add(ExpenseSeries);
            return plotModel1;
        }

        protected override string GetSql()
        {
            var fromUnix = UnixTimeConverter.ConvertBack(From ?? DateTime.MinValue);
            var toUnix = UnixTimeConverter.ConvertBack(To ?? DateTime.MaxValue);

            var dateFilter = $"AND t.datetime BETWEEN {fromUnix} AND {toUnix}";
            string str = this.TopCategory?.ID == null
                ? "parent_level = 0"
                : $"parent_left > {TopCategory.Left} AND parent_right < {TopCategory.Right} AND parent_level = 1";

            return string.Format(BaseSqlText, dateFilter, str);
        }

        private void UpdatePeriod(PeriodType type)
        {
            switch (type)
            {
                case PeriodType.Custom:
                    break;
                case PeriodType.Today:
                    {
                        From = DateTime.Today;
                        To = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.Yesterday:
                    {
                        From = DateTime.Today.AddDays(-1);
                        To = DateTime.Today.AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        From = startingDate.AddDays(-7);
                        To = startingDate.AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousMonth:
                    {
                        var today = DateTime.Today;

                        From = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1);
                        To = new DateTime(today.Year, today.Month, 1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.CurrentWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        From = startingDate;
                        To = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.CurrentMonth:
                    {
                        var today = DateTime.Today;

                        From = new DateTime(today.Year, today.Month, 1);
                        To = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousAndCurrentWeek:
                    {
                        DayOfWeek weekStart = DayOfWeek.Monday;
                        DateTime startingDate = DateTime.Today;

                        while (startingDate.DayOfWeek != weekStart)
                            startingDate = startingDate.AddDays(-1);

                        From = startingDate.AddDays(-7);
                        To = DateTime.Today.AddDays(1).AddMilliseconds(-1);
                    }
                    break;
                case PeriodType.PreviousAndCurrentMonth:
                    {
                        var today = DateTime.Today;

                        From = new DateTime(today.AddMonths(-1).Year, today.AddMonths(-1).Month, 1);
                        To = new DateTime(today.AddMonths(1).Year, today.AddMonths(1).Month, 1).AddMilliseconds(-1);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
