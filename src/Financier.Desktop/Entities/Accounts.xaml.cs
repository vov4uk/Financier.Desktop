using Financier.DataAccess.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for Accounts.xaml
    /// </summary>
    public partial class Accounts : UserControl
    {
        public RangeObservableCollection<Account> AccountsList { get; }
        public Accounts(RangeObservableCollection<Account> accounts)
        {
            AccountsList = accounts;
            DataContext = this;
            InitializeComponent();
        }
    }
}
