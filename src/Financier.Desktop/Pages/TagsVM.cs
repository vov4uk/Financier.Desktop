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
    public class TagsVM : TagBaseVM<TagModel>
    {
        public TagsVM(IFinancierDatabase db, IDialogWrapper dialogWrapper)
            : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => OpenTagDialogAsync<Tag>(0);

        protected override Task OnDelete(TagModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(TagModel item) => OpenTagDialogAsync<Tag>(item.Id ?? 0);

        protected override async Task RefreshData()
        {
            DbManual.ResetManuals(nameof(DbManual.Tag));
            await DbManual.SetupAsync(db);
            Entities = new ObservableCollection<TagModel>(DbManual.Tag.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
        }
    }
}
