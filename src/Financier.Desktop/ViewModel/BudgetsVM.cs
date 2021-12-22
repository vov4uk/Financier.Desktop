using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    public class BudgetsVM : EntityBaseVM<Budget>
    {
        public BudgetsVM(IEnumerable<Budget> items) : base(items) { }
    }
}
