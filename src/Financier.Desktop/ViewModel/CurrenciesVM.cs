using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    public class CurrenciesVM: EntityBaseVM<Currency>
    {
        public CurrenciesVM(IEnumerable<Currency> items) : base(items) {}
    }
}
