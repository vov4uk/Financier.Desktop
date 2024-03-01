using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.ViewModel.Dialog;
using Financier.Desktop.Views.Controls;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class LocationsVM : EntityBaseVM<LocationModel>
    {
        public LocationsVM(IFinancierDatabase db, IDialogWrapper dialogWrapper) : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => OpenLocationDialogAsync(0);

        protected override Task OnDelete(LocationModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(LocationModel item) => OpenLocationDialogAsync(item.Id ?? 0);

        protected override async Task RefreshData()
        {
            DbManual.ResetManuals(nameof(DbManual.Location));
            await DbManual.SetupAsync(db);
            Entities = new System.Collections.ObjectModel.ObservableCollection<LocationModel>(DbManual.Location.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
        }

        private async Task OpenLocationDialogAsync(int id)
        {
            Location selectedValue = await db.GetOrCreateAsync<Location>(id);
            LocationControlVM locationVm = new LocationControlVM(new LocationDto(selectedValue));

            var result = dialogWrapper.ShowDialog<LocationControl>(locationVm, 240, 300, nameof(Location));

            var updatedItem = result as LocationDto;
            if (updatedItem != null)
            {
                selectedValue.IsActive = updatedItem.IsActive;
                selectedValue.Address = updatedItem.Address;
                selectedValue.Title = updatedItem.Title;
                if (id == 0)
                {
                    selectedValue.Count = 0;
                }

                await db.InsertOrUpdateAsync(new[] { selectedValue });

                await RefreshData();
            }
        }
    }
}
