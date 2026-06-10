using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class SettingsDTO : BindableBase
    {

        private string exchangeRatesProvider;
        private string openExchangeRatesProviderAppId;
        private bool updateExchangeRatesOnStart;

        public bool IsAutoUpdateEnabled { get; init; }

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
    }
}
