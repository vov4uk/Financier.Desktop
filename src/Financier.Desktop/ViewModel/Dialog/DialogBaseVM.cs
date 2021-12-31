using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Financier.Desktop.ViewModel.Dialog
{
    public abstract class DialogBaseVM : BindableBase
    {
        private DelegateCommand _cancelCommand;

        private DelegateCommand _saveCommand;

        public event EventHandler RequestCancel;

        public event EventHandler RequestSave;

        public abstract object OnRequestSave();

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ??= new DelegateCommand(() => RequestCancel?.Invoke(this, EventArgs.Empty)); }
        }

        public DelegateCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new DelegateCommand(OnSave, CanSaveCommandExecute);
            }
        }

        protected virtual bool CanSaveCommandExecute()
        {
            return true;
        }

        void OnSave()
        {
            var output = OnRequestSave();
            RequestSave?.Invoke(output, EventArgs.Empty);
        }
    }
}
