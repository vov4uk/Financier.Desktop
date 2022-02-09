using Financier.Desktop.Data;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class LocationDialogVM : TagVM
    {
        private DelegateCommand _clearAddressCommand;

        public LocationDialogVM(LocationDto location) : base(location)
        {
            Entity = location;
        }

        public DelegateCommand ClearAddressCommand
        {
            get { return _clearAddressCommand ??= new DelegateCommand(() => { Entity.Address = default; }); }
        }

        public new LocationDto Entity { get; }
    }
}
