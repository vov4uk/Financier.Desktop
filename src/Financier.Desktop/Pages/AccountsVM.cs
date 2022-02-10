using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Helpers;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class AccountsVM : EntityBaseVM<AccountModel>
    {
        public AccountsVM(IFinancierDatabase db, IDialogWrapper dialogWrapper)
            : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => throw new System.NotImplementedException();

        protected override Task OnDelete(AccountModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(AccountModel item) => throw new System.NotImplementedException();

        protected override async Task RefreshData()
        {
            using var uow = db.CreateUnitOfWork();
            var accountRepo = uow.GetRepository<Account>();
            var items = await accountRepo.FindManyAsync(
                predicate: x => true,
                projection: acc => new AccountModel(acc),
                includes : x => x.Currency);

            Entities = new ObservableCollection<AccountModel>(items.OrderByDescending(x => x.IsActive).ThenBy(x => x.SortOrder));
        }
    }
}