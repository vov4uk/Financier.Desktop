using Financier.DataAccess.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for Payees.xaml
    /// </summary>
    public partial class Payees : UserControl
    {
        public RangeObservableCollection<Payee> PayeesList { get; }
        public Payees(RangeObservableCollection<Payee> payee)
        {
            PayeesList = payee;
            DataContext = this;
            InitializeComponent();
        }
    }
}
