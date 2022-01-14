using Financier.Desktop.Reports.ViewModel;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;

namespace Financier.Desktop.Views
{
    [ExcludeFromCodeCoverage]
    public partial class ByCategoryReportView : UserControl
    {

        public ByCategoryReportView()
        {
            InitializeComponent();
            Loaded += ReportView_Loaded;
        }

        private void ReportView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshReport(sender, e);
            SizeChanged += RefreshReport;
            DataContextChanged += (_, _) => this.RefreshReport(null, EventArgs.Empty);
        }

        private void RefreshReport(object sender, EventArgs e)
        {
            var context = DataContext as ByCategoryReportVM;
            if (context != null)
            {
                context.RefreshReport(PlotPresenter.RenderSize.Width);
            }
        }
    }
}
