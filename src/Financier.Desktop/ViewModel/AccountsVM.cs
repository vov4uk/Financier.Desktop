using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class AccountsVM : EntityBaseVM<Account>
    {
        public AccountsVM(IEnumerable<Account> items) : base(items) {}
    }
}