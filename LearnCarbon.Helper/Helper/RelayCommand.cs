using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LearnCarbon.Helper.Helper
{
    public class RelayCommand : ICommand
    {
        public Action Action { get; set; }
        public Func<bool> Validator { get; set; }
        public RelayCommand(Action _action, Func<bool> _validator)
        {
            Action = _action;
            Validator = _validator;
        }
        public RelayCommand(Action _action) : this(_action, null)
        {

        }
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if (Validator == null) return true;
            return Validator();
        }

        public void Execute(object parameter)
        {
            Action();
        }
    }
}
