using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GG40.Core.Abstraction
{
    public delegate void CallbackHandler<TData>(TData args);
    public delegate void CallbackHandler<TData1, TData2>(TData1 arg1, TData2 arg2);
    public delegate void CallbackHandler();
    public delegate Task<TResult> FunctionCallbackHandler<TResult>();
    public delegate Task<TResult> FunctionCallbackHandler<TArgs, TResult>(TArgs args);
    public delegate void ProgressCallbackHandler<TData>(TData arg);

    public class Command : ICommand
    {

        private Action<object> mAction;

        public event EventHandler CanExecuteChanged = (sender, e) => { };

        public Command(Action<object> action)
        {
            mAction = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            mAction(parameter);
        }
    }
}
