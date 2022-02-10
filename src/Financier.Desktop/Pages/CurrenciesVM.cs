using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;
using Financier.Desktop.Helpers;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class CurrenciesVM : EntityBaseVM<CurrencyModel>
    {
        public CurrenciesVM(IFinancierDatabase db, IDialogWrapper dialogWrapper) : base(db, dialogWrapper)
        {
        }

        protected override Task OnAdd() => throw new System.NotImplementedException();

        protected override Task OnDelete(CurrencyModel item) => throw new System.NotImplementedException();

        protected override Task OnEdit(CurrencyModel item) => throw new System.NotImplementedException();

        protected override Task RefreshData()
        {
            Entities = new ObservableCollection<CurrencyModel>(DbManual.Currencies.Where(x => x.Id.HasValue));
            return Task.CompletedTask;
        }
    }
}
