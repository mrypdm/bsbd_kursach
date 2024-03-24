using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace GuiClient.Commands;

public class FuncCommand<TParam>(Action<TParam> action, Func<bool> canExecute = null, bool allowNulls = false)
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

    public void Execute(object parameter)
    {
        if (parameter is not TParam && (parameter != null || !allowNulls))
        {
            MessageBox.Show($"{typeof(TParam).Name} was expected. But was {parameter?.GetType().Name ?? "<null>"}",
                "DEV_ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1);
        }

        try
        {
            action((TParam)parameter);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}