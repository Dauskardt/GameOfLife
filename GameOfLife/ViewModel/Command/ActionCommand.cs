using System;
using System.Windows.Input;

namespace GameOfLife.ViewModel.Command
{
    class ActionCommand : ICommand
    {
       
        public Predicate<object> CanExecuteFunc
        {
            get;
            set;
        }

        public Action<object> ExecuteFunc
        {
            get;
            set;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteFunc(parameter);
        }
        
        event EventHandler ICommand.CanExecuteChanged
        {
            add{} remove{}
        }
    }
}
