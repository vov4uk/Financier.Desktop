using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class SettingsDTO : BindableBase
    {
        private string exchangeRatesProvider;
        private bool updateExchangeRatesOnStart;
        private string openExchangeRatesProviderAppId;
        public string ExchangeRatesProvider
        {
            get => exchangeRatesProvider;
            set
            {
                if (exchangeRatesProvider != value)
                {
                    exchangeRatesProvider = value;
                    RaisePropertyChanged(nameof(ExchangeRatesProvider));
                }
            }
        }
        public bool UpdateExchangeRatesOnStart
        {
            get => updateExchangeRatesOnStart;
            set
            {
                if (updateExchangeRatesOnStart != value)
                {
                    updateExchangeRatesOnStart = value;
                    RaisePropertyChanged(nameof(UpdateExchangeRatesOnStart));
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
    }
}