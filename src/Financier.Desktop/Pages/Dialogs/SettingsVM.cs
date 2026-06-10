using Financier.Desktop.Data;
using Financier.Desktop.ViewModel.Dialog;

namespace Financier.Desktop.Pages.Dialogs
{
    public class SettingsVM : DialogBaseVM
    {
        bool _isOpenExchangeRatesProviderSelected;
        bool _isfreecurrencyratesSelected;
        bool _isMonobankProviderSelected;

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

        public bool IsFreeCurrencyRatesProviderSelected
        {
            get => _isfreecurrencyratesSelected;
            set
            {
                if (_isfreecurrencyratesSelected != value)
                {
                    _isfreecurrencyratesSelected = value;
                    RaisePropertyChanged(nameof(IsFreeCurrencyRatesProviderSelected));
                }
            }
        }
        public bool IsMonobankProviderSelected
        {
            get => _isMonobankProviderSelected;
            set
            {
                if (_isMonobankProviderSelected != value)
                {
                    _isMonobankProviderSelected = value;
                    RaisePropertyChanged(nameof(IsMonobankProviderSelected));
                }
            }
        }

        public SettingsVM(SettingsDTO entity)
        {
            this.Entity = entity;
            this.IsOpenExchangeRatesProviderSelected = entity.ExchangeRates.Provider == "openexchangerates.org";
            this.IsFreeCurrencyRatesProviderSelected = entity.ExchangeRates.Provider == "freecurrencyrates.com";
            this.IsMonobankProviderSelected = entity.ExchangeRates.Provider == "monobank.ua";
        }

        public SettingsDTO Entity { get; }

        public override object OnRequestSave()
        {
            if (IsOpenExchangeRatesProviderSelected)
            {
                Entity.ExchangeRates.Provider = "openexchangerates.org";
            }
            else if (IsFreeCurrencyRatesProviderSelected)
            {
                Entity.ExchangeRates.Provider = "freecurrencyrates.com";
                Entity.ExchangeRates.OpenExchangeRatesProviderAppId = "";
            }
            else if(IsMonobankProviderSelected)
            {
                Entity.ExchangeRates.Provider = "monobank.ua";
                Entity.ExchangeRates.OpenExchangeRatesProviderAppId = "";
            }
            return Entity;
        }
    }
}
