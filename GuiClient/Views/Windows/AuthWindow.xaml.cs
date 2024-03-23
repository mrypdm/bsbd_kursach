using System.Windows;
using GuiClient.ViewModels.Windows;

namespace GuiClient.Views.Windows;

/// <summary>
/// Interaction logic for AuthWindow.xaml
/// </summary>
public partial class AuthWindow : Window
{
    public AuthWindow()
    {
        InitializeComponent();
    }

    public AuthWindow(AuthWindowViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}