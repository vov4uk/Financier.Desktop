using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class ProjectsVM : EntityBaseVM<ProjectModel>
    {
        public ProjectsVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override Task RefreshData()
        {
            Entities = new System.Collections.ObjectModel.ObservableCollection<ProjectModel>(DbManual.Project.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
            return Task.CompletedTask;
        }
    }
}
