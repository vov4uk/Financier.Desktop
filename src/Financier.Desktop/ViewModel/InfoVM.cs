using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Financier.Desktop.ViewModel
{
    public class InfoVM : BindableBase
    {
        private DelegateCommand _exitCommand;
        public DelegateCommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                    _exitCommand = new DelegateCommand(Exit);

                return _exitCommand;
            }
        }

        private void Exit()
        {
            EventHandler handler = RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public event EventHandler RequestClose;

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }
    }
}
