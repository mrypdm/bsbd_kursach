using System.Windows;
using GuiClient.ViewModels;
using JetBrains.Annotations;

namespace GuiClient.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[UsedImplicitly]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(this, AuthUserControl.DataContext as AuthViewModel);
    }
}