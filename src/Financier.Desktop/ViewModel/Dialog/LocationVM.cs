using Financier.DataAccess.Data;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class LocationVM : EntityWithTitleVM
    {
        private DelegateCommand _clearAddressCommand;

        private string address;

        public LocationVM(Location location)
        {
            Id = location.Id;
            Title = location.Name;
            Address = location.Address;
            IsActive = location.IsActive == true;
        }

        public LocationVM()
        {
        }

        public DelegateCommand ClearAddressCommand
        {
            get { return _clearAddressCommand ??= new DelegateCommand(() => { Address = default; }); }
        }

        public string Address
        {
            get => address;
            set
            {
                address = value;
                RaisePropertyChanged(nameof(Address));
            }
        }
    }
}
