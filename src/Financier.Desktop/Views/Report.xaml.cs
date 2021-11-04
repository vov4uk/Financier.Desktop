using Financier.Desktop.Reports.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {

        public Report()
        {
            InitializeComponent();
            Loaded += Report_Loaded;
            SizeChanged += Report_Loaded;
        }

        private void Report_Loaded(object sender, RoutedEventArgs e)
        {
            ((ReportVM)DataContext).RefreshReport(PlotPresenter.RenderSize.Width);
        }
    }
}
