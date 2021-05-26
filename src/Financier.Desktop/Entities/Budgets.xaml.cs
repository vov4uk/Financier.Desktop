using Financier.DataAccess.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for Budgets.xaml
    /// </summary>
    public partial class Budgets : UserControl
    {
        public RangeObservableCollection<Budget> BudgetsList { get; }
        public Budgets(RangeObservableCollection<Budget> currencies)
        {
            BudgetsList = currencies;
            DataContext = this;
            InitializeComponent();
        }
    }
}
