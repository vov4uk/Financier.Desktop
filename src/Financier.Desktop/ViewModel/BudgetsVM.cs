using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class BudgetsVM : EntityBaseVM<Budget>
    {
        public BudgetsVM(IEnumerable<Budget> items) : base(items) {}
    }
}
