using FinancistoAdapter.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FinancierDesktop.Entities
{
    /// <summary>
    /// Interaction logic for ExchangeRates.xaml
    /// </summary>
    public partial class ExchangeRates : UserControl
    {
        public ReadOnlyObservableCollection<ExchangeRate> ExchangeRatesList { get; }
        public ExchangeRates(ReadOnlyObservableCollection<ExchangeRate> rates)
        {
            ExchangeRatesList = rates;
            DataContext = this;
            InitializeComponent();
        }
    }
}
