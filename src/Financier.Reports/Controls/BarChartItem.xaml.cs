using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace Financier.Reports.Controls
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class BarChartItem : UserControl
    {
        public BarChartItem(double coef, ReportRow row)
        {
            InitializeComponent();
            InitUI(coef, row);
        }
        void InitUI(double coef, ReportRow row)
        {
            Title.Text = row.Title;

            var positiveAbs = Math.Abs(row.TotalPositiveAmount);
            var negativeAbs = Math.Abs(row.TotalNegativeAmount);

            if (row.GetAbsoluteMax() == positiveAbs)
            {
                Bar1.Fill = new SolidColorBrush { Color = Colors.Green };
                Bar1.Width = Math.Max(15, positiveAbs * coef);
                Label1.Text = $"+{positiveAbs / 100.0:F2}{row.CurrencySign}";

                if (negativeAbs > 0)
                {
                    Stack2.Visibility = System.Windows.Visibility.Visible;
                    Bar2.Fill = new SolidColorBrush { Color = Colors.Orange };
                    Bar2.Width = Math.Max(15, negativeAbs * coef);
                    Label2.Text = $"-{positiveAbs / 100.0:F2}{row.CurrencySign}";
                }

            }
            else if (row.GetAbsoluteMax() == negativeAbs)
            {
                Bar1.Fill = new SolidColorBrush { Color = Colors.Orange };
                Bar1.Width = Math.Max(15, negativeAbs * coef);
                Label1.Text = $"-{negativeAbs / 100.0:F2}{row.CurrencySign}";

                if (positiveAbs > 0)
                {
                    Stack2.Visibility = System.Windows.Visibility.Visible;
                    Bar2.Fill = new SolidColorBrush { Color = Colors.Green };
                    Bar2.Width = Math.Max(15, positiveAbs * coef);
                    Label2.Text = $"+{positiveAbs / 100.0:F2}{row.CurrencySign}";
                }
            }
        }
    }
}
