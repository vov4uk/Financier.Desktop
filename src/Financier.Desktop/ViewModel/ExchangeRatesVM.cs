using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class ExchangeRatesVM : EntityBaseVM<CurrencyExchangeRate>
    {
        public ExchangeRatesVM(IEnumerable<CurrencyExchangeRate> items) : base(items) {}
    }
}
