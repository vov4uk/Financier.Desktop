using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.DataAccess.Data;
using Financier.Desktop.Helpers;
using Financier.Desktop.Pages;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class PayeesVM : TagBaseVM<PayeeModel>
    {
        public PayeesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper) : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => OpenTagDialogAsync<Payee>(0);

        protected override Task OnDelete(PayeeModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(PayeeModel item) => OpenTagDialogAsync<Payee>((int)item.Id);

        protected override async Task RefreshData()
        {
            DbManual.ReseManuals(nameof(DbManual.Payee));
            await DbManual.SetupAsync(db);
            Entities = new ObservableCollection<PayeeModel>(DbManual.Payee.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
        }
    }
}
