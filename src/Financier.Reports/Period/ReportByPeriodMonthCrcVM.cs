using Financier.Common.Attribute;
using Financier.DataAccess.Abstractions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.Collections.Generic;

namespace Financier.Reports
{
    [Header("By months")]
    public class ReportByPeriodMonthCrcVM : BaseReportVM<ReportByPeriodMonthCrcModel>
    {
        private const string BaseSqlText = @" /* ReportByPeriodMonthCrcVM */
select
    tx.date_year as date_year,
    tx.date_month as date_month,
    round(tx.credit_sum, 2) as credit_sum,
    round(tx.debit_sum, 2) as debit_sum,
    round(tx.credit_sum - tx.debit_sum, 2) as saldo
from (
select
    date_year,
    date_month,
    sum( case when from_amount > 0 then (case when {0} = 1 then from_amount else from_amount_default_crr end ) else 0 end) / 100.00 as credit_sum,
    sum( case when from_amount < 0 then - (case when {0} = 1 then from_amount else from_amount_default_crr end ) else 0 end) / 100.00  as debit_sum
from v_report_transactions 
where to_account_id = 0 and (payee_id > 0 or category_id > 0 or project_id > 0)
     /*FILTERS*/
{1}
     /*FILTERS*/
group by
    date_year,
    date_month
order by
    date_year,
    date_month
) tx";

        public ReportByPeriodMonthCrcVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {

        }

        protected override string GetSql()
        {
            string str = string.Empty;
            if (CurentCurrency.Id.HasValue)
            {
                str = string.Format("and from_account_crc_id = {0}", CurentCurrency.Id);
            }
            string standartTrnFilter = GetStandartTrnFilter();
            if (standartTrnFilter != string.Empty)
            {
                str = str + " and " + standartTrnFilter;
            }
            return string.Format(BaseSqlText, CurentCurrency.Id.HasValue ? 1 : 0, str);
        }

        protected override PlotModel GetPlotModel(List<ReportByPeriodMonthCrcModel> list)
        {
            var saldo = new LineSeries
            {
                Title = "Saldo",
                RenderInLegend = true,
                LabelFormatString = "{1}",
                MarkerType = MarkerType.Circle,
            };

            var debit = new BarSeries
            {
                XAxisKey = "Value",
                YAxisKey = "Category",
                Title = "Outcome",
                RenderInLegend = true,
            };

            var credit = new BarSeries
            {
                Title = "Income",
                XAxisKey = "Value",
                YAxisKey = "Category",
                RenderInLegend = true,
            };

            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Key = "Value",
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
            };

            var legend = new Legend
            {
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0,
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.RightMiddle
            };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Category",
                LabelField = "PeriodDesr",
            };

            categoryAxis.ItemsSource = list;

            int i = 0;
            foreach (var c in list)
            {
                debit.ActualItems.Add(new BarItem { Value = c.DebitSum ?? 0 });
                credit.ActualItems.Add(new BarItem { Value = c.CreditSum ?? 0 });
                saldo.Points.Add(new DataPoint(i++, c.Saldo ?? 0));
            }

            var plotModel = new PlotModel();

            plotModel.Series.Add(credit);
            plotModel.Series.Add(debit);
            plotModel.Series.Add(saldo);
            plotModel.Legends.Add(legend);
            plotModel.Axes.Add(valueAxis);
            plotModel.Axes.Add(categoryAxis);
            return plotModel;
        }
    }
}