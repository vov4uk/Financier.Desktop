using Financier.Common.Entities;
using Financier.Desktop.Data;
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

        public SettingsVM(SettingsDTO entity)
        {
            this.Entity = entity;
            this.SelectedProvider = entity.ExchangeRates.Provider;
        }

        public SettingsDTO Entity { get; }

        public override object OnRequestSave()
        {
            Entity.ExchangeRates.Provider = SelectedProvider;

            if (!IsOpenExchangeRatesProviderSelected)
            {
                Entity.ExchangeRates.OpenExchangeRatesProviderAppId = "";
            }
            return Entity;
        }
    }
}
