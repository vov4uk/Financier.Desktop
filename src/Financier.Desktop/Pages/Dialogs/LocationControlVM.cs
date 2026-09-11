using Financier.Desktop.Data;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class LocationControlVM : TagControlVM
    {
        private DelegateCommand _clearAddressCommand;

        public LocationControlVM(LocationDto location) : base(location)
        {
            Entity = location;
        }

        public DelegateCommand ClearAddressCommand => _clearAddressCommand ??= new DelegateCommand(() => { Entity.Address = default; });

        public new LocationDto Entity { get; }
    }
}
