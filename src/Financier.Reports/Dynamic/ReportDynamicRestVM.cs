using Financier.Common.Attribute;
using Financier.DataAccess.Abstractions;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Reports
{
    [Header("Balance dynamics")]
    public class ReportDynamicRestVM : BaseReportVM<ReportDynamicRestModel>
    {

        // group by week
//        SELECT cr.year                                           AS year,
//               cr.month AS month,
//               cr.week AS week,
//               Round((SELECT Sum(from_amount_default_crr)
//                      FROM   v_report_transactions trn
//                      WHERE Date(trn.datetime / 1000, 'unixepoch') <= cr.date
//                            AND to_account_id = 0
//                     AND category_id != -1) / 100.00, 2) AS total
//FROM(SELECT date_year                               AS year,
//               date_month AS month,
//               date_week AS week,
//               Max(Date(datetime / 1000, 'unixepoch')) AS date
//        FROM v_report_transactions
//        WHERE  1 = 1
//               AND((date_year = 2020
//                       AND date_month >= 9 )
//                      OR date_year > 2020 )
//               AND((date_year = 2021
//                       AND date_month <= 1 )
//                      OR date_year< 2021 )
//        GROUP BY date_year,
//                  date_month,
//                  date_week) cr
//ORDER  BY year,
//          month



        private const string BaseSqlText = @" /* ReportDynamicRestVM */
SELECT cr.year  AS year,
       cr.month AS month,
       cr.day   AS day,
       Round(  (
               SELECT Sum(from_amount_default_crr)
               FROM   v_report_transactions trn
               WHERE  Date(trn.datetime / 1000, 'unixepoch') <= cr.date
               AND    to_account_id = 0
               AND    category_id != -1) / 100.00, 2 ) AS total
FROM   (
                       SELECT DISTINCT Date(datetime / 1000, 'unixepoch') AS date,
                                       date_year      AS year,
                                       date_month     AS month,
                                       date_day       AS day
                       FROM            v_report_transactions
                       WHERE           1 = 1 {0}
                                       /* FILTER */
       ) cr
ORDER BY year, month, day";

        public ReportDynamicRestVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override string GetSql()
        {
            string standartTrnFilter = GetStandartTrnFilter();
            return string.Format(BaseSqlText, !string.IsNullOrEmpty(standartTrnFilter) ? " and " + standartTrnFilter : string.Empty);
        }

        protected override SafePlotModel GetPlotModel(List<ReportDynamicRestModel> list)
        {
            var model = new SafePlotModel();
            var dateTimeAxis = new DateTimeAxis();

            var linearAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
            };
            var lineSeries = new LineSeries
            {
                Color = OxyColor.FromArgb(255, 78, 154, 6),
                MarkerFill = OxyColor.FromArgb(255, 78, 154, 6),
                MarkerStroke = OxyColors.ForestGreen,
                MarkerType = MarkerType.Plus,
                StrokeThickness = 1
            };

            foreach (var item in list.OrderBy(x => x.Year).ThenBy(x => x.Month).ThenBy(x => x.Day))
            {
                lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(new DateTime(item.Year, item.Month, item.Day), item.Total ?? 0));
            }

            model.Axes.Add(dateTimeAxis);
            model.Axes.Add(linearAxis);
            model.Series.Add(lineSeries);

            return model;
        }
    }
}