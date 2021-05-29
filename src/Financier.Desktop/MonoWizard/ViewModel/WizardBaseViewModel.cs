using Prism.Mvvm;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public abstract class WizardBaseViewModel : BindableBase
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
                RaisePropertyChanged(nameof(IsCurrentPage));
            }
        }
    }
}
