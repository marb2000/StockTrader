using System;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace StockTraderRI.Infrastructure
{
    public class RelayCommand<T> : ICommand, IDisposable
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;
        private Dispatcher _dispatcher;

        public RelayCommand(Action<T> execute) :
            this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = new Action<T>(execute);

            if (canExecute != null)
                _canExecute = new Func<T, bool>(canExecute);
        }

        public event EventHandler _canExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    _canExecuteChanged += value;
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (_canExecute != null)
                {
                    _canExecuteChanged -= value;
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            try
            {
                if (_dispatcher == null)
                    _dispatcher = Dispatcher.CurrentDispatcher;

                if (_canExecute == null)
                    return true;

                if (parameter == null)
                    return _canExecute(default(T));

                return _canExecute((T)parameter);
            }
            catch (Exception)
            {
            }
            return false;
        }

        public virtual void Execute(object parameter)
        {
            try
            {
                if (_dispatcher == null)
                    _dispatcher = Dispatcher.CurrentDispatcher;

                var val = parameter;
                if (parameter != null && parameter.GetType() != typeof(T))
                {
                    if (parameter is IConvertible)
                    {
                        val = Convert.ChangeType(parameter, typeof(T), null);
                    }
                }

                if (_execute != null)
                {
                    if (val == null)
                        _execute(default(T));
                    else
                        _execute((T)val);
                }
            }
            catch (Exception)
            {
            }
        }

        public void UpdateCanExecuteState()
        {
            EventHandler handler = Interlocked.CompareExchange(ref _canExecuteChanged, null, null);
            if (handler != null)
            {
                if (_dispatcher == null || Thread.CurrentThread == _dispatcher.Thread)
                    handler.Invoke(this, EventArgs.Empty);
                else
                    _dispatcher.Invoke(handler, this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_canExecuteChanged != null)
            {
                var delgates = _canExecuteChanged.GetInvocationList().ToList();
                foreach (var del in delgates)
                {
                    CanExecuteChanged -= (EventHandler)del;
                }
            }
        }
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action<object> execute) :
            base(execute)
        {
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute) :
            base(execute, canExecute)
        {
        }
    }
}
