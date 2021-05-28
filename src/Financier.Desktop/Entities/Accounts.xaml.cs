using Financier.DataAccess.Data;
using System.Collections.Generic;
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
        public Accounts(List<Account> accounts)
        {
            AccountsList = new RangeObservableCollection<Account>(accounts);
            DataContext = this;
            InitializeComponent();
        }
    }
}
