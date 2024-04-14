using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Domain;

namespace GuiClient.Commands;

public class AsyncActionCommand(Func<Task> action, Func<bool> canExecute = null) : ICommand
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
        try
        {
            await action();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Logging.Logger.Error(e, "{Message}", e.Message);
        }
    }
}