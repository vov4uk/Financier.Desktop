using FinancistoAdapter.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FinancierDesktop.Entities
{
    /// <summary>
    /// Interaction logic for Locations.xaml
    /// </summary>
    public partial class Locations : UserControl
    {
        public ReadOnlyObservableCollection<Location> LocationsList { get; }
        public Locations(ReadOnlyObservableCollection<Location> locations)
        {
            LocationsList = locations;
            DataContext = this;
            InitializeComponent();
        }
    }
}
