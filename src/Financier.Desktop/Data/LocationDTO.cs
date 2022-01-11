using Financier.DataAccess.Data;

namespace Financier.Desktop.Data
{
    public class LocationDto : EntityWithTitleDto
    {
        private string address;

        public LocationDto(Location location) : base(location)
        {
            Address = location.Address;
        }

        public LocationDto(string title, bool isActive, string address)
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
