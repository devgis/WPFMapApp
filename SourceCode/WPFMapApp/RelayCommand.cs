using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFMapApp
{
    public class RelayCommand : ICommand
    {
        private Action<MyPoint> _action;
        public RelayCommand(Action<MyPoint> action)
        {
            _action = action;
        }
        #region ICommand Members
        public bool CanExecute(MyPoint parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(MyPoint parameter)
        {
            _action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is MyPoint)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                _action(new MyPoint());
            }
            else
            {
                _action((MyPoint)parameter);
            }
            
        }
        #endregion
    }
}
