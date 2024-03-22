using System.Windows;

namespace GuiClient.ViewModels.Windows;

public abstract class WindowViewModel<TWindow>(TWindow control) : BaseViewModel<TWindow>(control)
    where TWindow : Window
{
    /// <inheritdoc cref="Window.ShowDialog" />
    public bool? ShowDialog()
    {
        return Control.ShowDialog();
    }
}