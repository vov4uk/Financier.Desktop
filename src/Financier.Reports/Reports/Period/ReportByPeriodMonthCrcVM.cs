using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.Collections.Generic;

namespace Financier.Reports.Reports
{
    [Header("По месяцам")]
    public class ReportByPeriodMonthCrcVM : BaseReportVM<ReportByPeriodMonthCrcModel>
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
                FROM     v_report_transactions
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

        public ReportByPeriodMonthCrcVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override string GetSql()
        {
            string str = string.Empty;
            if (CurentCurrency.ID.HasValue)
            {
                str = string.Format("and from_account_crc_id = {0}", CurentCurrency.ID);
            }
            string standartTrnFilter = GetStandartTrnFilter();
            if (standartTrnFilter != string.Empty)
            {
                str = str + " and " + standartTrnFilter;
            }
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
"\r\n                from v_report_transactions " +
"\r\n                where to_account_id = 0 and (payee_id > 0 or category_id > 0 or project_id > 0)" +
"\r\n                        {1} /*FILTERS*/" +
"\r\n                group by" +
"\r\n                    date_year," +
"\r\n                    date_month" +
"\r\n                order by" +
"\r\n                    date_year," +
"\r\n                    date_month" +
"\r\n                ) tx" +
"\r\n        ",
CurentCurrency.ID.HasValue ? 1 : 0,
str);
        }

        protected override void SetupSeries(List<ReportByPeriodMonthCrcModel> list)
        {
            var model = new PlotModel();

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
                Title = "Debit",
                RenderInLegend = true,
            };

            var credit = new BarSeries
            {
                Title = "Credit",
                XAxisKey = "Value",
                YAxisKey = "Category",
                RenderInLegend = true,
            };

            var categoryAxis = new CategoryAxis
            {
                Position = AxisPosition.Bottom,
                Key = "Category",
                LabelField = "PeriodDesr",
                ItemsSource = list
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

            int i = 0;
            foreach (var c in list)
            {
                debit.ActualItems.Add(new BarItem { Value = c.DebitSum ?? 0 });
                credit.ActualItems.Add(new BarItem { Value = c.CreditSum ?? 0 });
                saldo.Points.Add(new DataPoint(i++, c.Saldo ?? 0));
            }

            model.Series.Add(credit);
            model.Series.Add(debit);
            model.Series.Add(saldo);
            model.Legends.Add(legend);
            model.Axes.Add(categoryAxis);
            model.Axes.Add(valueAxis);

            PlotModel = model;
        }
    }
}