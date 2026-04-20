using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Content.Helper
{
    /*
     This class is a simple implementation of the ICommand interface, which allows us to bind commands in our ViewModels to methods in our Views.


    The class tamplate is for when we want to pass a parameter to the method.
    Example:
    IncreaseQuantityCommand = new RelayCommand<CartItem>(IncreaseQuantity);
     */
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }
    }

    /*
     * The class is for when we don't have parameters to pass.
    Example:
    SelectAdminCommand = new RelayCommand(SetAdmin);
     */

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
