namespace Financier.Desktop.Tests.Pages.Dialog
{
    using System.Collections.Generic;
    using Financier.Common.Entities;
    using Financier.Desktop.Data;
    using Financier.Desktop.Pages.Dialogs;
    using Xunit;

    public class SettingsVMTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_NullOrEmptyAppId_RemainsUnchanged(string appId)
        {
            var entity = CreateEntity(appId: appId);
            var vm = new SettingsVM(entity);
            Assert.Equal(appId, vm.Entity.ExchangeRates.OpenExchangeRatesProviderAppId);
        }

        [Fact]
        public void Constructor_PlainTextAppId_IsRetainedAsIs()
        {
            // TryDecrypt falls back to returning the input when it is not a valid DPAPI blob
            var entity = CreateEntity(appId: "plain-text-key");
            var vm = new SettingsVM(entity);
            Assert.Equal("plain-text-key", vm.Entity.ExchangeRates.OpenExchangeRatesProviderAppId);
        }

        [Fact]
        public void Constructor_SetsEntityProperty()
        {
            var entity = CreateEntity();
            var vm = new SettingsVM(entity);
            Assert.Same(entity, vm.Entity);
        }

        [Theory]
        [InlineData(ExchangeRatesProviders.None)]
        [InlineData(ExchangeRatesProviders.Monobank)]
        [InlineData(ExchangeRatesProviders.OpenExchangeRates)]
        [InlineData(ExchangeRatesProviders.FreeCurrencyRates)]
        public void Constructor_SetsSelectedProvider_FromEntity(ExchangeRatesProviders provider)
        {
            var entity = CreateEntity(provider: provider);
            var vm = new SettingsVM(entity);
            Assert.Equal(provider, vm.SelectedProvider);
        }

        [Theory]
        [InlineData(ExchangeRatesProviders.None)]
        [InlineData(ExchangeRatesProviders.Monobank)]
        [InlineData(ExchangeRatesProviders.FreeCurrencyRates)]
        public void IsOpenExchangeRatesProviderSelected_WhenNotOpenExchangeRates_ReturnsFalse(ExchangeRatesProviders provider)
        {
            var vm = new SettingsVM(CreateEntity(provider: provider));
            Assert.False(vm.IsOpenExchangeRatesProviderSelected);
        }

        [Fact]
        public void IsOpenExchangeRatesProviderSelected_WhenOpenExchangeRates_ReturnsTrue()
        {
            var vm = new SettingsVM(CreateEntity(provider: ExchangeRatesProviders.OpenExchangeRates));
            Assert.True(vm.IsOpenExchangeRatesProviderSelected);
        }

        [Fact]
        public void OnRequestSave_ReturnsEntity()
        {
            var entity = CreateEntity();
            var vm = new SettingsVM(entity);

            var result = vm.OnRequestSave();

            Assert.Same(entity, result);
        }

        [Theory]
        [InlineData(ExchangeRatesProviders.None)]
        [InlineData(ExchangeRatesProviders.Monobank)]
        [InlineData(ExchangeRatesProviders.OpenExchangeRates)]
        [InlineData(ExchangeRatesProviders.FreeCurrencyRates)]
        public void OnRequestSave_SetsEntityProvider_FromSelectedProvider(ExchangeRatesProviders provider)
        {
            var entity = CreateEntity();
            var vm = new SettingsVM(entity);
            vm.SelectedProvider = provider;

            vm.OnRequestSave();

            Assert.Equal(provider, entity.ExchangeRates.Provider);
        }

        [Theory]
        [InlineData(ExchangeRatesProviders.None)]
        [InlineData(ExchangeRatesProviders.Monobank)]
        [InlineData(ExchangeRatesProviders.FreeCurrencyRates)]
        public void OnRequestSave_WithNonOpenExchangeRatesProvider_ClearsAppId(ExchangeRatesProviders provider)
        {
            var entity = CreateEntity(provider: provider, appId: "some-api-key");
            var vm = new SettingsVM(entity);
            vm.SelectedProvider = provider;

            vm.OnRequestSave();

            Assert.Equal(string.Empty, entity.ExchangeRates.OpenExchangeRatesProviderAppId);
        }

        [Fact]
        public void OnRequestSave_WithOpenExchangeRatesProviderAndEmptyAppId_LeavesAppIdEmpty()
        {
            var entity = CreateEntity(provider: ExchangeRatesProviders.OpenExchangeRates, appId: "");
            var vm = new SettingsVM(entity);

            vm.OnRequestSave();

            Assert.Equal(string.Empty, entity.ExchangeRates.OpenExchangeRatesProviderAppId);
        }

        [Fact]
        public void OnRequestSave_WithOpenExchangeRatesProviderAndNonEmptyAppId_EncryptsAppId()
        {
            const string plainText = "my-secret-app-id";
            var entity = CreateEntity(provider: ExchangeRatesProviders.OpenExchangeRates, appId: plainText);
            var vm = new SettingsVM(entity);

            vm.OnRequestSave();

            Assert.NotEqual(plainText, entity.ExchangeRates.OpenExchangeRatesProviderAppId);
            Assert.NotEmpty(entity.ExchangeRates.OpenExchangeRatesProviderAppId);
        }

        [Fact]
        public void SelectedProvider_WhenChanged_RaisesPropertyChangedForSelfAndIsOpenExchangeRatesProviderSelected()
        {
            var vm = new SettingsVM(CreateEntity(provider: ExchangeRatesProviders.None));
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedProvider = ExchangeRatesProviders.OpenExchangeRates;

            Assert.Contains(nameof(vm.SelectedProvider), raised);
            Assert.Contains(nameof(vm.IsOpenExchangeRatesProviderSelected), raised);
        }

        [Fact]
        public void SelectedProvider_WhenSetToSameValue_DoesNotRaisePropertyChanged()
        {
            var vm = new SettingsVM(CreateEntity(provider: ExchangeRatesProviders.Monobank));
            var raised = new List<string>();
            vm.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

            vm.SelectedProvider = ExchangeRatesProviders.Monobank;

            Assert.Empty(raised);
        }

        private static SettingsDto CreateEntity(
            ExchangeRatesProviders provider = ExchangeRatesProviders.None,
            string appId = "") =>
            new SettingsDto
            {
                ExchangeRates = new SettingsExchangeRates
                {
                    Provider = provider,
                    OpenExchangeRatesProviderAppId = appId,
                },
            };
    }
}
