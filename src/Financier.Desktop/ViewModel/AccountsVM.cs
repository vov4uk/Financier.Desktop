using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    public class AccountsVM : EntityBaseVM<Account>
    {
        public AccountsVM(IEnumerable<Account> items) : base(items) {}
    }
}