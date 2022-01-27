using Financier.DataAccess.Abstractions;
using Financier.Reports.Common;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Financier.Reports.Reports
{
    [Header("Dynamics of expenses-incomes")]
    public class ReportDynamicDebitCretitPayeeVM : BaseReportVM<ReportDynamicDebitCretitPayeeModel>
    {
        private const string BaseSqlText = @" /* ReportDynamicDebitCretitPayeeVM */
 select
    tx.date_year as date_year,
    tx.date_month as date_month,
    round(tx.total / 100.00, 2) as total
from
    (select
        trn.date_year,
        trn.date_month,
        trn.category_id,
        sum(case when {0} = 1 then from_amount else from_amount_default_crr end) as total
    from v_report_transactions trn
    where (payee_id > 0 or category_id > 0 or project_id > 0)
    /*FILTERS*/
        {1}
    /*FILTERS*/
    group by
        trn.date_year,
        trn.date_month ) tx
order by
    tx.date_year,
    tx.date_month";

        public ReportDynamicDebitCretitPayeeVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override string GetSql()
        {
            long? categoryId;
            int hasCategory;
            if (!Payee.ID.HasValue)
            {
                categoryId = Category.ID;
                hasCategory = categoryId.HasValue ? 1 : 0;
            }
            else
            {
                hasCategory = 1;
            }

            if (hasCategory == 0)
            {
                MessageBox.Show("Please select category!");
                return string.Empty;
            }
            string str = string.Empty;
            categoryId = CurentCurrency.ID;
            if (categoryId.HasValue)
            {
                str = string.Format(" and from_account_crc_id = {0}", CurentCurrency.ID);
            }
            string standartTrnFilter = GetStandartTrnFilter();
            if (standartTrnFilter != string.Empty)
            {
                str = str + " and " + standartTrnFilter;
            }
            categoryId = CurentCurrency.ID;
            return string.Format(BaseSqlText, categoryId.HasValue ? 1 : 0, str);
        }

        protected override PlotModel GetPlotModel(List<ReportDynamicDebitCretitPayeeModel> list)
        {
            var model = new PlotModel();

            var dateTimeAxis = new DateTimeAxis();

            var valueAxis = new LinearAxis
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
            };

            // TODO - add title (selected payee + category)
            var values = new LineSeries
            {
                RenderInLegend = false,
                LabelFormatString = "{1}",
                MarkerType = MarkerType.Circle,
            };

            foreach (var item in list.OrderBy(x => x.Year).ThenBy(x => x.Month))
            {
                values.Points.Add(DateTimeAxis.CreateDataPoint(new DateTime((int)item.Year, (int)item.Month, 1), item.Total ?? 0));
            }

            model.Series.Add(values);

            model.Axes.Add(valueAxis);
            model.Axes.Add(dateTimeAxis);


           return model;
        }
    }
}