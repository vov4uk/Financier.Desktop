using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    public class ExchangeRatesVM : EntityBaseVM<CurrencyExchangeRate>
    {
        public ExchangeRatesVM(IEnumerable<CurrencyExchangeRate> items) : base(items) { }
    }
}
