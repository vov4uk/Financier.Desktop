using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Financier.Desktop.ViewModel
{
    [ExcludeFromCodeCoverage]
    public class InfoVM : BindableBase
    {
        private DelegateCommand _exitCommand;
        private string _text;

        public event EventHandler RequestClose;

        public DelegateCommand ExitCommand => _exitCommand ??= new DelegateCommand(() => RequestClose?.Invoke(this, EventArgs.Empty));

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }
    }
}
