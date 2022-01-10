using Financier.DataAccess.Data;

namespace Financier.Desktop.Data
{
    public class LocationDTO : EntityWithTitleDTO
    {
        private string address;

        public LocationDTO(Location location) : base(location)
        {
            Address = location.Address;
        }

        public LocationDTO(string title, bool isActive, string address)
            : base(title, isActive)
        {
            this.Address = address;
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
