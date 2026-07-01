using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Financier.Common.Entities;
using Financier.Common.Localization;

namespace Financier.Desktop.ViewModel.Dialog
{
    public record CurrencyTemplateItem(bool IsNewCurrency, List<string> Template, string DisplayName);

    [ExcludeFromCodeCoverage]
    public class AddCurrencyControlVM : DialogBaseVM
    {
        private CurrencyTemplateItem _selectedItem;

        public AddCurrencyControlVM()
        {
            var items = DbManual.AllCurrencies.Select(t => new CurrencyTemplateItem(false, t, $"{t[0]} ({t[1]})")).ToList();
            items.Insert(0, new CurrencyTemplateItem(true, null, LocalizationService.Instance["new_currency"]));

            Items = items;
            SelectedItem = items[0];
        }

        public IReadOnlyList<CurrencyTemplateItem> Items { get; }

        public CurrencyTemplateItem SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; RaisePropertyChanged(nameof(SelectedItem)); }
        }

        public override object OnRequestSave() => SelectedItem;
    }
}
