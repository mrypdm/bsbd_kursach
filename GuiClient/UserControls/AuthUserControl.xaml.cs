using System.Windows;
using System.Windows.Controls;
using DatabaseClient.Users;
using GuiClient.Windows;

namespace GuiClient.UserControls;

/// <summary>
/// Interaction logic for AuthUserControl.xaml
/// </summary>
public partial class AuthUserControl : UserControl
{
    public AuthUserControl()
    {
        InitializeComponent();
    }

    public User CurrentUser { get; private set; }

    private void Authenticate(object sender, RoutedEventArgs e)
    {
        CurrentUser = null;
        ChangePasswordButton.IsEnabled = false;

        var authWindow = new AuthWindow();
        var result = authWindow.ShowDialog();

        if (result != true)
        {
            return;
        }

        CurrentUser = authWindow.User;
        ChangePasswordButton.IsEnabled = true;
        UserLabel.Text = $"{CurrentUser.Role.ToString()}/{CurrentUser.Login}";
    }

    private void ChangePassword(object sender, RoutedEventArgs e)
    {
    }
}