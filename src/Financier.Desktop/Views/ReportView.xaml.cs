using Financier.Desktop.Reports.ViewModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Financier.Desktop.Views
{
    [ExcludeFromCodeCoverage]
    public partial class ReportView : UserControl
    {

        public ReportView()
        {
            InitializeComponent();
            Loaded += RefreshReport;
            SizeChanged += RefreshReport;
        }

        private void RefreshReport(object sender, RoutedEventArgs e)
        {
            ((ReportVM)DataContext).RefreshReport(PlotPresenter.RenderSize.Width);
        }
    }
}
