using System.Windows;
using DatabaseClient.Users;
using JetBrains.Annotations;

namespace GuiClient.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
[UsedImplicitly]
public partial class MainWindow : Window
{
    private static User _currentUser;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void AuthButton_OnClick(object sender, RoutedEventArgs e)
    {
        _currentUser = null;

        var authWindow = new AuthWindow();
        var result = authWindow.ShowDialog();

        if (result != true)
        {
            return;
        }

        _currentUser = authWindow.User;
    }
}