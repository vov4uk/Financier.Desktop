using Financier.DataAccess.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for Locations.xaml
    /// </summary>
    public partial class Locations : UserControl
    {
        public RangeObservableCollection<Location> LocationsList { get; }
        public Locations(RangeObservableCollection<Location> locations)
        {
            LocationsList = locations;
            DataContext = this;
            InitializeComponent();
        }
    }
}
