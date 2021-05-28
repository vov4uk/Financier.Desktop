using Financier.DataAccess.View;
using System.Collections.Generic;
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
        public Blotter(List<TransactionsView> transactions)
        {
            InitializeComponent();
            Transactions = new ObservableCollection<TransactionsView>(transactions);
            DataContext = this;
        }
    }
}
