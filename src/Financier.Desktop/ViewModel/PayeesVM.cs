using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class PayeesVM: EntityBaseVM<Payee>
    {
        public PayeesVM(IEnumerable<Payee> items) : base(items) {}
    }
}
