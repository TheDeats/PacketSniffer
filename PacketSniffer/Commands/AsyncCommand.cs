using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketSniffer
{
    public class AsyncCommand<T> : AsyncCommandBase<T>
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<bool> _canExecute;

        public AsyncCommand(Func<T, Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        protected override async Task ExecuteInternalAsync(T parameter = default)
        {
            await base.ExecuteInternalAsync();
            await _execute(parameter);
        }
    }

    public class AsyncCommand : AsyncCommandBase<object>
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        protected override async Task ExecuteInternalAsync(object parameter = default)
        {
            await base.ExecuteInternalAsync();
            await _execute();
        }
    }
}
