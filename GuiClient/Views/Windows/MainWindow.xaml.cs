using System.Windows;
using GuiClient.ViewModels.UserControls;
using GuiClient.ViewModels.Windows;
using JetBrains.Annotations;

namespace GuiClient.Views.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[UsedImplicitly]
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(this, AuthUserControl.DataContext as AuthControlViewModel);
    }
}