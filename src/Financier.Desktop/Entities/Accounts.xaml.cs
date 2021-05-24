using FinancistoAdapter.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FinancierDesktop.Entities
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts : UserControl
    {
        public ReadOnlyObservableCollection<Account> AccountsList { get; }
        public Accounts(ReadOnlyObservableCollection<Account> accounts)
        {
            AccountsList = accounts;
            DataContext = this;
            InitializeComponent();
        }
    }
}
