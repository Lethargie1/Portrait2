using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PortraitCrusher
{
    public class RelayCommand<T> : ICommand
    {


        readonly Action<T> _execute = null;
        readonly Predicate<T> _canExecute = null;

        /// Initializes a new instance of 
        //Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
        //will always return true
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }


        /// Creates a new command.
        // <param name="execute">The execution logic.</param>
        // <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }


        //Defines the method that determines whether the command can execute in its current state.
        //
        //<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        //true if this command can be executed; otherwise, false.
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }


        //Occurs when changes occur that affect whether or not the command should execute.

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        //Defines the method to be called when the command is invoked.

        //<param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to <see langword="null" />.</param>
        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }
}
