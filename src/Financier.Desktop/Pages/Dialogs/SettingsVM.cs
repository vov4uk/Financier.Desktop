using Financier.Common.Entities;
using Financier.Desktop.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.ViewModel.Dialog;

namespace Financier.Desktop.Pages.Dialogs
{
    public class SettingsVM : DialogBaseVM
    {
        ExchangeRatesProviders _providerSelected;

        public bool IsOpenExchangeRatesProviderSelected
        {
            get => SelectedProvider == ExchangeRatesProviders.OpenExchangeRates;
        }

        public ExchangeRatesProviders SelectedProvider
        {
            get => _providerSelected;
            set
            {
                if (_providerSelected != value)
                {
                    _providerSelected = value;
                    RaisePropertyChanged(nameof(SelectedProvider));
                    RaisePropertyChanged(nameof(IsOpenExchangeRatesProviderSelected));
                }
            }
        }

        public SettingsVM(SettingsDto entity)
        {
            this.Entity = entity;
            this.SelectedProvider = entity.ExchangeRates.Provider;
            Entity.ExchangeRates.OpenExchangeRatesProviderAppId = SettingsProtection.TryDecrypt(Entity.ExchangeRates.OpenExchangeRatesProviderAppId);
        }

        public SettingsDto Entity { get; }

        public override object OnRequestSave()
        {
            Entity.ExchangeRates.Provider = SelectedProvider;

            if (!IsOpenExchangeRatesProviderSelected)
            {
                Entity.ExchangeRates.OpenExchangeRatesProviderAppId = "";
            }
            else if (!string.IsNullOrEmpty(Entity.ExchangeRates.OpenExchangeRatesProviderAppId))
            {
                Entity.ExchangeRates.OpenExchangeRatesProviderAppId = SettingsProtection.Encrypt(Entity.ExchangeRates.OpenExchangeRatesProviderAppId);
            }
            return Entity;
        }
    }
}
