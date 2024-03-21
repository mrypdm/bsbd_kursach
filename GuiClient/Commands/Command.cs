using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GuiClient.Commands;

public class Command(Action action, Func<bool> canExecute = null) : ICommand
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
        action();
    }
}