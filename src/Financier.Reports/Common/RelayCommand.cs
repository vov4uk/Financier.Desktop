using System;
using System.Diagnostics;
using System.Windows.Input;

namespace fcrd
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
          : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this._execute = execute != null ? execute : throw new ArgumentNullException(nameof(execute));
            this._canExecute = canExecute;
        }

        [DebuggerStepThrough]
        public bool CanExecute(object parameter) => this._canExecute == null || this._canExecute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter) => this._execute(parameter);
    }
}