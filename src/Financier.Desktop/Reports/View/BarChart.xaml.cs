using Financier.Desktop.Reports.Model;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Financier.Desktop.Reports
{
    /// <summary>
    /// Logique d'interaction pour BarChart.xaml
    /// </summary>
    public partial class BarChart : UserControl
    {
        public BarChart(List<ReportRow> values, double maxWidth)
        {
            InitializeComponent();
            InitUI(values, maxWidth);
        }

        void InitUI(List<ReportRow> values, double maxWidth)
        {
            double maxValue = values.Select(x => x.GetAbsoluteMax()).Max();
            double coef = maxWidth / maxValue;

            ChartPresenter.Children.Clear();
            long totalPositive = 0;
            long totalNegative = 0;
            foreach (var value in values.OrderByDescending(x => x.GetAbsoluteMax()))
            {
                totalPositive += value.TotalPositiveAmount;
                totalNegative += value.TotalNegativeAmount;
                ChartPresenter.Children.Add(new BarChartItem(coef, value));
            }

            Summary.Text = $"{totalNegative / 100.0:F2}{values.First().CurrencySign} | +{totalPositive / 100.0:F2}{values.First().CurrencySign}";
        }
    }
}
