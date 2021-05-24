using FinancistoAdapter.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FinancierDesktop.Entities
{
    /// <summary>
    /// Interaction logic for Currencies.xaml
    /// </summary>
    public partial class Currencies : UserControl
    {
        public ReadOnlyObservableCollection<Currency> CurrenciesList { get; }
        public Currencies(ReadOnlyObservableCollection<Currency> currencies)
        {
            CurrenciesList = currencies;
            DataContext = this;
            InitializeComponent();
        }
    }
}
