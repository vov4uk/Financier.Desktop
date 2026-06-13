using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Desktop.ViewModel.Dialog
{
    [ExcludeFromCodeCoverage]
    public abstract class DialogBaseVM : BindableBase
    {
        private DelegateCommand _cancelCommand;
        private DelegateCommand _saveCommand;

        public event EventHandler RequestCancel;
        public event EventHandler RequestSave;

        public DelegateCommand CancelCommand => _cancelCommand ??= new DelegateCommand(() => RequestCancel?.Invoke(this, EventArgs.Empty));
        public DelegateCommand SaveCommand => _saveCommand ??= new DelegateCommand(OnSave, CanSaveCommandExecute);

        public abstract object OnRequestSave();
        protected virtual bool CanSaveCommandExecute() => true;

        private void OnSave()
        {
            var output = OnRequestSave();
            RequestSave?.Invoke(output, EventArgs.Empty);
        }
    }
}
