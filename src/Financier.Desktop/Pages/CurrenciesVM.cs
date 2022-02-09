using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class CurrenciesVM : EntityBaseVM<CurrencyModel>
    {
        public CurrenciesVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override Task RefreshData()
        {
            Entities = new System.Collections.ObjectModel.ObservableCollection<CurrencyModel>(DbManual.Currencies.Where(x => x.Id.HasValue));
            return Task.CompletedTask;
        }
    }
}
