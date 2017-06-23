using System;
using System.Windows.Input;

namespace KomicAheGao
{
    public class CommandBase : ICommand
    {
        private readonly Action<object> _command;
        private readonly Func<object, bool> _canExecute;

        public CommandBase(Action<object> command, Func<object, bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            _canExecute = canExecute;
            _command = command;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _command(parameter);
        }
    }
}
