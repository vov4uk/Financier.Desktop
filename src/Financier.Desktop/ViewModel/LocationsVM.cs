using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class LocationsVM : EntityBaseVM<Location>
    {
        public LocationsVM(IEnumerable<Location> items) : base(items) {}
    }
}
