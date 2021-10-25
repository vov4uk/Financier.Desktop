using Financier.DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class LocationVM : BindableBase
    {
        private DelegateCommand _cancelCommand;

        private DelegateCommand _clearTitleCommand;

        private DelegateCommand _clearAddressCommand;

        private DelegateCommand _saveCommand;

        private int id;
        private bool isActive;
        private string title;
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

        public event EventHandler RequestCancel;

        public event EventHandler RequestSave;

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ??= new DelegateCommand(() => RequestCancel?.Invoke(this, EventArgs.Empty)); }
        }

        public DelegateCommand ClearTitleCommand
        {
            get { return _clearTitleCommand ??= new DelegateCommand(() => { Title = default; }); }
        }

        public DelegateCommand ClearAddressCommand
        {
            get { return _clearAddressCommand ??= new DelegateCommand(() => { Address = default; }); }
        }
                public DelegateCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new DelegateCommand(() => RequestSave?.Invoke(this, EventArgs.Empty));
            }
        }

        public int Id
        {
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
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

        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                RaisePropertyChanged(nameof(IsActive));
            }
        }
    }
}
