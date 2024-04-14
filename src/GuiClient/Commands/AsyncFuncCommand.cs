using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Domain;

namespace GuiClient.Commands;

public class AsyncFuncCommand<TParam>(
    Func<TParam, Task> action,
    Func<TParam, bool> canExecute = null,
    bool allowNulls = false)
    : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        if (canExecute == null)
        {
            return true;
        }

        if (parameter is not TParam && parameter != null)
        {
            throw new InvalidOperationException(
                $"{typeof(TParam).Name} was expected. But was {parameter.GetType().Name}");
        }

        return canExecute((TParam)parameter);
    }

    public async void Execute(object parameter)
    {
        try
        {
            if (parameter is not TParam && (parameter != null || !allowNulls))
            {
                throw new InvalidOperationException(
                    $"{typeof(TParam).Name} was expected. But was {parameter?.GetType().Name ?? "<null>"}");
            }

            await action((TParam)parameter);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logging.Logger.Error(e, "{Message}", e.Message);
        }
    }
}