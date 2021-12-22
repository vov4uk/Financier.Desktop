using System.Collections.Generic;
using Financier.DataAccess.Data;

namespace Financier.Desktop.ViewModel
{
    public class LocationsVM : EntityBaseVM<Location>
    {
        public LocationsVM(IEnumerable<Location> items) : base(items) { }
    }
}
