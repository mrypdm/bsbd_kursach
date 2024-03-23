using System.Windows;
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
    }

    public MainWindow(MainWindowViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }
}