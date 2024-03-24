using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace GuiClient.Commands;

public class ActionCommand(Action action, Func<bool> canExecute = null) : ICommand
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
        try
        {
            action();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}