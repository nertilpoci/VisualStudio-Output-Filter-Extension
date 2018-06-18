using System;
using System.Windows.Input;

namespace FilteredOutputWindowVSX.Tools
{
    public class RelayCommand : ICommand
    {
        private readonly Action _execute = null;
        private readonly Func<bool> _canExecute = null;

        public RelayCommand(Action a_execute)
            : this(a_execute, null)
        {
        }

        public RelayCommand(Action a_execute, Func<bool> a_canExecute)
        {
            _execute = a_execute ?? throw new ArgumentNullException(nameof(a_execute));
            _canExecute = a_canExecute;
        }

        public bool CanExecute(object a_parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object a_parameter)
        {
            _execute();
        }
    }
}
