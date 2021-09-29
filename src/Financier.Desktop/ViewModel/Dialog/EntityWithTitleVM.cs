using Financier.DataAccess.Data;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class EntityWithTitleVM : BindableBase
    {
        private DelegateCommand _cancelCommand;

        private DelegateCommand _clearTitleCommand;

        private DelegateCommand _saveCommand;

        private int id;
        private bool isActive;
        private string title;

        public EntityWithTitleVM(Project proj)
        {
            Id = proj.Id;
            Title = proj.Title;
            IsActive = proj.IsActive == true;
        }

        public EntityWithTitleVM(Payee payee)
        {
            Id = payee.Id;
            Title = payee.Title;
            IsActive = payee.IsActive == true;
        }

        public EntityWithTitleVM()
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
