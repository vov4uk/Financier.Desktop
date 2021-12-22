using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    public class PayeesVM: EntityBaseVM<Payee>
    {
        public PayeesVM(IEnumerable<Payee> items) : base(items) { }
    }
}
