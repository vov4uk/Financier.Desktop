using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Pages;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class ProjectsVM : TagBaseVM<ProjectModel>
    {
        public ProjectsVM(IFinancierDatabase db, IDialogWrapper dialogWrapper) : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => OpenTagDialogAsync<Project>(0);

        protected override Task OnDelete(ProjectModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(ProjectModel item) => OpenTagDialogAsync<Project>(item.Id ?? 0);

        protected override async Task RefreshData()
        {
            DbManual.ResetManuals(nameof(DbManual.Project));
            await DbManual.SetupAsync(db);
            Entities = new ObservableCollection<ProjectModel>(DbManual.Project.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
        }
    }
}
