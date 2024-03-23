using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiClient.Commands;

public class AsyncFuncCommand<TParam>(Func<TParam, Task> action, Func<bool> canExecute = null, bool allowNulls = false)
    : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return canExecute?.Invoke() ?? true;
    }

    public async void Execute(object parameter)
    {
        if (parameter is TParam || (parameter == null && allowNulls))
        {
            await action((TParam)parameter);
        }
        else
        {
            throw new InvalidOperationException(
                $"{typeof(TParam).Name} was expected. But was {parameter?.GetType().Name ?? "<null>"}");
        }
    }
}