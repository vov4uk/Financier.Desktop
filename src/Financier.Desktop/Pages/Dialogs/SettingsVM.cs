using Financier.Desktop.Data;
using Financier.Desktop.ViewModel.Dialog;

namespace Financier.Desktop.Pages.Dialogs
{
    public class SettingsVM : DialogBaseVM
    {
        bool _isOpenExchangeRatesProviderSelected;
        bool _isfreecurrencyratesSelected;

        public bool IsOpenExchangeRatesProviderSelected
        {
            get => _isOpenExchangeRatesProviderSelected;
            set
            {
                if (_isOpenExchangeRatesProviderSelected != value)
                {
                    _isOpenExchangeRatesProviderSelected = value;
                    RaisePropertyChanged(nameof(IsOpenExchangeRatesProviderSelected));
                }
            }
        }

        public bool IsfreecurrencyratesSelected
        {
            get => _isfreecurrencyratesSelected;
            set
            {
                if (_isfreecurrencyratesSelected != value)
                {
                    _isfreecurrencyratesSelected = value;
                    RaisePropertyChanged(nameof(IsfreecurrencyratesSelected));
                }
            }
        }

        public SettingsVM(SettingsDTO entity)
        {
            this.Entity = entity;
            this.IsOpenExchangeRatesProviderSelected = entity.ExchangeRatesProvider == "openexchangerates.org";
            this.IsfreecurrencyratesSelected = entity.ExchangeRatesProvider == "freecurrencyrates.com";
        }

        public SettingsDTO Entity { get; }

        public override object OnRequestSave()
        {
            if (IsOpenExchangeRatesProviderSelected)
            {
                Entity.ExchangeRatesProvider = "openexchangerates.org";
            }
            else
            {
                Entity.ExchangeRatesProvider = "freecurrencyrates.com";
                Entity.OpenExchangeRatesProviderAppId = "";
            }
            return Entity;
        }
    }
}
