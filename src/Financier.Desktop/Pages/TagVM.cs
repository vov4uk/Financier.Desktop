using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.ViewModel;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views.Controls;
using System.Threading.Tasks;

namespace Financier.Desktop.Pages
{
    public abstract class TagBaseVM<TEntity> : EntityBaseVM<TEntity>
        where TEntity : BaseModel, new()
    {
        protected TagBaseVM(IFinancierDatabase db, IDialogWrapper dialogWrapper) : base(db, dialogWrapper)
        {
        }

        protected async Task OpenTagDialogAsync<T>(int e)
            where T : Tag, new()
        {
            T selectedEntity = await db.GetOrCreateAsync<T>(e);
            TagControlVM context = new TagControlVM(new TagDto(selectedEntity));

            var result = dialogWrapper.ShowDialog<TagControl>(context, 180, 300, typeof(T).Name);

            var updatedItem = result as TagDto;
            if (updatedItem != null)
            {
                selectedEntity.IsActive = updatedItem.IsActive;
                selectedEntity.Title = updatedItem.Title;

                await db.InsertOrUpdateAsync(new[] { selectedEntity });
                await RefreshData();
            }
        }
    }
}
