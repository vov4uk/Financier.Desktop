using System;
using Financier.Common.Entities;
using Financier.Common.Localization;
using Prism.Mvvm;

namespace Financier.Desktop.Data
{
    public class SettingsDto : BindableBase, ICloneable
    {
        public SettingsGeneralDto General { get; set; } = new SettingsGeneralDto();
        public SettingsExchangeRates ExchangeRates { get; set; } = new SettingsExchangeRates();

        public object Clone()
        {
            var clone = new SettingsDto
            {
                General = (SettingsGeneralDto)General.Clone(),
                ExchangeRates = (SettingsExchangeRates)ExchangeRates.Clone()
            };
            return clone;
        }
    }

    public class SettingsGeneralDto : BindableBase, ICloneable
    {
        private bool checkForUpdatesOnStart;
        private Language language;

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

        public Language Language
        {
            get => language;
            set
            {
                if (language != value)
                {
                    language = value;
                    RaisePropertyChanged(nameof(Language));
                }
            }
        }

        public object Clone()
        {
            return new SettingsGeneralDto
            {
                CheckForUpdatesOnStart = CheckForUpdatesOnStart,
                Language = Language
            };
        }
    }

    public class SettingsExchangeRates: BindableBase, ICloneable
    {
        private ExchangeRatesProviders exchangeRatesProvider;
        private string openExchangeRatesProviderAppId;
        private bool updateOnStart;



        public ExchangeRatesProviders Provider
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

        public object Clone()
        {
            return new SettingsExchangeRates
            {
                Provider = Provider,
                OpenExchangeRatesProviderAppId = OpenExchangeRatesProviderAppId,
                UpdateOnStart = UpdateOnStart
            };
        }
    }
}
