using FinancistoAdapter.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FinancierDesktop.Entities
{
    /// <summary>
    /// Interaction logic for Payees.xaml
    /// </summary>
    public partial class Payees : UserControl
    {
        public ReadOnlyObservableCollection<Payee> PayeesList { get; }
        public Payees(ReadOnlyObservableCollection<Payee> payee)
        {
            PayeesList = payee;
            DataContext = this;
            InitializeComponent();
        }
    }
}
