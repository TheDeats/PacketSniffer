using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PacketSniffer
{
    public abstract class AsyncCommandBase<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private bool _isExecuting;

        protected AsyncCommandBase(){ }

        public bool CanExecute(object parameter = null)
        {
            return !_isExecuting;
        }

        public void Execute(object parameter)
        {
            ExecuteAsync((T)parameter);
        }

        public async Task ExecuteAsync(T parameter = default)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    RaiseCanExecuteChangedInternal();
                    await ExecuteInternalAsync(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChangedInternal();
        }

        protected virtual Task ExecuteInternalAsync(T parameter = default)
        {
            return Task.CompletedTask;
        }

        protected void RaiseCanExecuteChangedInternal()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
