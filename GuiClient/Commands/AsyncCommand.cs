using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiClient.Commands;

public class AsyncCommand(Func<Task> action, Func<bool> canExecute = null) : ICommand
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
        await action();
    }
}