using Financier.DataAccess.View;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for Blotter.xaml
    /// </summary>
    public partial class Blotter : UserControl
    {
        public ObservableCollection<TransactionsView> Transactions { get; }
        public Blotter(ObservableCollection<TransactionsView> transactions)
        {
            Transactions = transactions;
            DataContext = this;
            InitializeComponent();
        }
    }
}
