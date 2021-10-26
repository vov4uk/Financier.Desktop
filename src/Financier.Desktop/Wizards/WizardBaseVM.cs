using Prism.Mvvm;

namespace Financier.Desktop.MonoWizard.ViewModel
{
    public abstract class WizardBaseVM : BindableBase
    {
        bool _isCurrentPage;
        public bool IsCurrentPage
        {
            get => _isCurrentPage;
            set
            {
                if (value == _isCurrentPage)
                    return;

                _isCurrentPage = value;
                RaisePropertyChanged(nameof(IsCurrentPage));
            }
        }

        public abstract string Title { get; }

        public abstract bool IsValid();
    }
}
