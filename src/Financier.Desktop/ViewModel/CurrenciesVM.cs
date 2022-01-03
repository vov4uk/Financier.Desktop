using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class CurrenciesVM: EntityBaseVM<Currency>
    {
        public CurrenciesVM(IEnumerable<Currency> items) : base(items) {}
    }
}
