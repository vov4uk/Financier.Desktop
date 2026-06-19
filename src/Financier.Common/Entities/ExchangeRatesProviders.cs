using System.ComponentModel;

namespace Financier.Common.Entities
{
    public enum ExchangeRatesProviders
    {
        None,
        [Description("monobank.ua")]
        Monobank,
        [Description("openexchangerates.org")]
        OpenExchangeRates,
        [Description("freecurrencyrates.com")]
        FreeCurrencyRates
    }
}
