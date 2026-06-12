using System.Threading.Tasks;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Localization;
using Financier.Desktop.ViewModel;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views.Controls;

namespace Financier.Desktop.Pages
{
    public abstract class TagBaseVM<TEntity> : EntityBaseVM<TEntity>
        where TEntity : BaseModel, new()
    {
        protected TagBaseVM(IFinancierDatabase db, IDialogWrapper dialogWrapper, LocalizationManager localizationManager)
            : base(db, dialogWrapper, localizationManager)
        {
        }

        protected async Task OpenTagDialogAsync<T>(int e)
            where T : Tag, new()
        {
            T selectedEntity = await db.GetOrCreateAsync<T>(e);
            TagControlVM context = new TagControlVM(new TagDTO(selectedEntity))
            {
                LocalizationManager = localizationManager
            };

            var result = dialogWrapper.ShowDialog<TagControl>(context, 180, 300, typeof(T).Name);

            var updatedItem = result as TagDTO;
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
