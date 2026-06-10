using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class SettingsDTO : BindableBase
    {
        public SettingsGeneralDTO General { get; set; } = new SettingsGeneralDTO();
        public SettingsExchangeRates ExchangeRates { get; set; } = new SettingsExchangeRates();
    }

    public class SettingsGeneralDTO : BindableBase
    {
        private bool checkForUpdatesOnStart;

        public bool CheckForUpdatesOnStart
        {
            get => checkForUpdatesOnStart;
            set
            {
                if (checkForUpdatesOnStart != value)
                {
                    checkForUpdatesOnStart = value;
                    RaisePropertyChanged(nameof(CheckForUpdatesOnStart));
                }
            }
        }
    }

    public class SettingsExchangeRates: BindableBase
    {
        private string exchangeRatesProvider;
        private string openExchangeRatesProviderAppId;
        private bool updateOnStart;



        public string Provider
        {
            get => exchangeRatesProvider;
            set
            {
                if (exchangeRatesProvider != value)
                {
                    exchangeRatesProvider = value;
                    RaisePropertyChanged(nameof(Provider));
                }
            }
        }
        public string OpenExchangeRatesProviderAppId
        {
            get => openExchangeRatesProviderAppId;
            set
            {
                if (openExchangeRatesProviderAppId != value)
                {
                    openExchangeRatesProviderAppId = value;
                    RaisePropertyChanged(nameof(OpenExchangeRatesProviderAppId));
                }
            }
        }

        public bool UpdateOnStart
        {
            get => updateOnStart;
            set
            {
                if (updateOnStart != value)
                {
                    updateOnStart = value;
                    RaisePropertyChanged(nameof(UpdateOnStart));
                }
            }
        }
    }
}
