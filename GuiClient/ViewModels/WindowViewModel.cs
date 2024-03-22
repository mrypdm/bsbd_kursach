using System.Windows;

namespace GuiClient.ViewModels;

public abstract class WindowViewModel<TWindow>(TWindow control) : BaseViewModel<TWindow>(control)
    where TWindow : Window
{
    /// <inheritdoc cref="Window.ShowDialog"/>
    public bool? ShowDialog() => Control.ShowDialog();
}