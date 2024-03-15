using System;
using System.Windows.Input;

namespace Applivery.Desktop.Core.Utils
{
    /// <summary>
    /// RelayCommand implementation taken from 
    /// https://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        /// <summary>
        /// This implementation ensures that, if somebody hookt to this CanExecuteChanged event, the CommandManager
        /// will notify them on its RequerySuggested, which is triggered "the CommandManager detects conditions that 
        /// might change the ability of a command to execute.", which is mainly on any input event in the application (mouse click, focus change). 
        /// As another benefit, CommandManager.RequerySuggested uses weak eventing and does not need to be unregistered.
        /// In order to raise the CanExecuteChanged manually, call CommandManager.InvalidateRequerySuggested
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion // ICommand Members
    }
}
