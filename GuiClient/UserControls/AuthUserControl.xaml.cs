using System;
using System.Windows;
using System.Windows.Controls;
using DatabaseClient.Users;
using Domain;
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
        if (CurrentUser != null)
        {
            LogOff();
            return;
        }

        var authWindow = new AuthWindow();
        if (authWindow.ShowDialog() != true)
        {
            return;
        }

        LogIn(authWindow.User);
    }

    private void ChangePassword(object sender, RoutedEventArgs e)
    {
        if (CurrentUser == null)
        {
            Logging.Logger.Error("Something wrong. User can change password while unauthorized");
            throw new InvalidOperationException("Attempt to change password for null user");
        }
        
        var authWindow = new AuthWindow(true, CurrentUser);
        if (authWindow.ShowDialog() != true)
        {
            return;
        }

        LogOff();
    }

    private void LogIn(User user)
    {
        AuthenticateButton.Content = "Log Off";
        CurrentUser = user;
        ChangePasswordButton.IsEnabled = true;
        UserLabel.Text = $"{CurrentUser.Role}/{CurrentUser.Login}";
    }

    private void LogOff()
    {
        AuthWindow.LogOff();
        AuthenticateButton.Content = "Log In";
        CurrentUser = null;
        ChangePasswordButton.IsEnabled = false;
        UserLabel.Text = string.Empty;
    }
}