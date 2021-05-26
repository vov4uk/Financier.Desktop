using Financier.DataAccess.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for Currencies.xaml
    /// </summary>
    public partial class Currencies : UserControl
    {
        public RangeObservableCollection<Currency> CurrenciesList { get; }
        public Currencies(RangeObservableCollection<Currency> currencies)
        {
            CurrenciesList = currencies;
            DataContext = this;
            InitializeComponent();
        }
    }
}
