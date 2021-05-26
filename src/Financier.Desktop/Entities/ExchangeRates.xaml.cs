using Financier.DataAccess.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for ExchangeRates.xaml
    /// </summary>
    public partial class ExchangeRates : UserControl
    {
        public RangeObservableCollection<CurrencyExchangeRate> ExchangeRatesList { get; }
        public ExchangeRates(RangeObservableCollection<CurrencyExchangeRate> rates)
        {
            ExchangeRatesList = rates;
            DataContext = this;
            InitializeComponent();
        }
    }
}
