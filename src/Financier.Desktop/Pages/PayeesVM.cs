using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class PayeesVM : EntityBaseVM<PayeeModel>
    {
        public PayeesVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override async Task RefreshData()
        {
            DbManual.ReseManuals(nameof(DbManual.Payee));
            await DbManual.SetupAsync(financierDatabase);
            Entities = new System.Collections.ObjectModel.ObservableCollection<PayeeModel>(DbManual.Payee.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
        }
    }
}
