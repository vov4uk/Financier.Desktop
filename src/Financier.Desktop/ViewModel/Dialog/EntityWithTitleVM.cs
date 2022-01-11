using Financier.DataAccess.Data;
using Prism.Commands;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class EntityWithTitleVM : DialogBaseVM
    {
        private DelegateCommand _clearTitleCommand;

        public EntityWithTitleVM(EntityWithTitleDto entity)
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

        public DelegateCommand ClearTitleCommand
        {
            get { return _clearTitleCommand ??= new DelegateCommand(() => { Title = default; }); }
        }

        public EntityWithTitleDto Entity { get; }
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
