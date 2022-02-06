using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Financier.Common.Entities;
using Financier.Common.Model;
using Financier.DataAccess.Abstractions;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class LocationsVM : EntityBaseVM<LocationModel>
    {
        public LocationsVM(IFinancierDatabase financierDatabase) : base(financierDatabase)
        {
        }

        protected override Task RefreshData()
        {
            Entities = new System.Collections.ObjectModel.ObservableCollection<LocationModel>(DbManual.Location.Where(x => x.Id > 0).OrderByDescending(x => x.IsActive).ThenBy(x => x.Id));
            return Task.CompletedTask;
        }
    }
}
