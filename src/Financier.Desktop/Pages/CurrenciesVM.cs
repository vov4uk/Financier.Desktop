using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Localization;
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
    public class CurrenciesVM : EntityBaseVM<CurrencyModel>
    {
        public CurrenciesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper)
            : base(db, dialogWrapper)
        {
        }

        protected override async Task OnAdd()
        {
            var addVm = new AddCurrencyControlVM();
            var selection = dialogWrapper.ShowDialog<AddCurrencyControl>(addVm, 400, 340, LocalizationService.Instance.currencies)
                            as CurrencyTemplateItem;

            if (selection == null)
                return;

            if (selection.IsNewCurrency)
            {
                await OpenCurrencyDialogAsync(0);
            }
            else
            {
                await SaveFromTemplateAsync(selection.Template);
            }
        }

        protected override Task OnDelete(CurrencyModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(CurrencyModel item) => OpenCurrencyDialogAsync(item.Id ?? 0);

        protected override async Task RefreshData()
        {
            DbManual.ResetManuals(nameof(DbManual.Currencies));
            await DbManual.SetupAsync(db);
            Entities = new ObservableCollection<CurrencyModel>(DbManual.Currencies.Where(x => x.Id.HasValue));
        }

        private async Task OpenCurrencyDialogAsync(int id)
        {
            Currency entity = await db.GetOrCreateAsync<Currency>(id);
            var dto = new CurrencyDto(entity);
            if (id == 0)
                dto.UpdateExchangeRate = true;

            var vm = new CurrencyControlVM(dto);
            var result = dialogWrapper.ShowDialog<CurrencyControl>(vm, 340, 440, LocalizationService.Instance.currency);

            var updated = result as CurrencyDto;
            if (updated == null)
                return;

            entity.Title = updated.Title;
            entity.Name = updated.Name;
            entity.Symbol = updated.Symbol;
            entity.IsDefault = updated.IsDefault;
            entity.UpdateExchangeRate = updated.UpdateExchangeRate;
            entity.Decimals = updated.Decimals;
            entity.DecimalSeparator = updated.DecimalSeparator;
            entity.GroupSeparator = updated.GroupSeparator;
            entity.SymbolFormat = updated.SymbolFormat.ToString();
            entity.NumberFormat = updated.NumberFormat;

            await db.InsertOrUpdateAsync(new[] { entity });
            await RefreshData();
        }

        private async Task SaveFromTemplateAsync(System.Collections.Generic.List<string> template)
        {
            Currency entity = await db.GetOrCreateAsync<Currency>(0);
            entity.Name = template[0];
            entity.Title = template[1];
            entity.Symbol = template[2];
            entity.Decimals = int.TryParse(template[3], out int d) ? System.Math.Clamp(d, 0, 3) : 2;
            entity.DecimalSeparator = template[4];
            entity.GroupSeparator = template[5];
            entity.IsActive = true;
            entity.IsDefault = !DbManual.Currencies.Any(x => x.Id.HasValue);

            await db.InsertOrUpdateAsync(new[] { entity });
            await RefreshData();
        }
    }
}
