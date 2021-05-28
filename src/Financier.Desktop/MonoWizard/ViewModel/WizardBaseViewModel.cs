using MonoWizard.Helpers;

namespace MonoWizard.ViewModel
{
    public abstract class WizardBaseViewModel : NotifyModelBase
    {
        public abstract string Title { get; }

        public abstract bool IsValid();

        bool _isCurrentPage;
        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set
            {
                if (value == _isCurrentPage)
                    return;

                _isCurrentPage = value;
                OnPropertyChanged("IsCurrentPage");
            }
        }
    }
}
