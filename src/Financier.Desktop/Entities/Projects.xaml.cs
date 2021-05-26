using Financier.DataAccess.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for Projects.xaml
    /// </summary>
    public partial class Projects : UserControl
    {
        public RangeObservableCollection<Project> ProjectsList { get; }
        public Projects(RangeObservableCollection<Project> projects)
        {
            ProjectsList = projects;
            DataContext = this;
            InitializeComponent();
        }
    }
}
