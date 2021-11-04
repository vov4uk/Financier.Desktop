using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Financier.Desktop.ViewModel.Dialog
{
    public class DialogBaseVM : BindableBase
    {
        private DelegateCommand _cancelCommand;

        private DelegateCommand _saveCommand;

        public event EventHandler RequestCancel;

        public event EventHandler RequestSave;

        public DelegateCommand CancelCommand
        {
            get { return _cancelCommand ??= new DelegateCommand(() => RequestCancel?.Invoke(this, EventArgs.Empty)); }
        }

        public DelegateCommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new DelegateCommand(() => RequestSave?.Invoke(this, EventArgs.Empty), CanSaveCommandExecute);
            }
        }

        protected virtual bool CanSaveCommandExecute()
        {
            return true;
        }
    }
}
