using FinancistoAdapter.Entities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace FinancierDesktop.Entities
{
    /// <summary>
    /// Interaction logic for Projects.xaml
    /// </summary>
    public partial class Projects : UserControl
    {
        public ReadOnlyObservableCollection<Project> ProjectsList { get; }
        public Projects(ReadOnlyObservableCollection<Project> projects)
        {
            ProjectsList = projects;
            DataContext = this;
            InitializeComponent();
        }
    }
}
