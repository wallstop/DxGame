using System;
using System.Windows.Input;
using DxCore.Core.Utils.Validate;

namespace AnimationEditor.Core
{
    public sealed class RelayCommand : ICommand
    {
        private Predicate<object> CheckExecute { get; }
        private Action<object> ExecuteAction { get; }

        public RelayCommand() : this(_ => { }) {}

        public RelayCommand(Action<object> execute) : this(execute, _ => true) {}

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            Validate.Hard.IsNotNull(execute);
            Validate.Hard.IsNotNull(canExecute);
            ExecuteAction = execute;
            CheckExecute = canExecute;
        }

        public bool CanExecute(object parameter) => CheckExecute(parameter);

        public void Execute(object parameter) => ExecuteAction(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}