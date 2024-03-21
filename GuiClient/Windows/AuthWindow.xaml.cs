using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using DatabaseClient.Contexts;
using DatabaseClient.Users;

namespace GuiClient.Windows;

/// <summary>
/// Interaction logic for AuthWindow.xaml
/// </summary>
public partial class AuthWindow : Window
{
    public AuthWindow()
        : this(false, null)
    {
    }

    public AuthWindow(bool changePassword, [NotNull] User user)
    {
        User = user;

        InitializeComponent();

        if (changePassword)
        {
            LoginBox.Text = user.Login;
            LoginBox.IsEnabled = false;
            ChangePasswordDock.Visibility = Visibility.Visible;
        }
        else
        {
            AuthenticateButton.Visibility = Visibility.Visible;
        }
    }

    public static void LogOff()
    {
        DatabaseContext.LogOff();
    }

    public User User { get; private set; }

    private async void Authenticate(object sender, RoutedEventArgs e)
    {
        var login = LoginBox.Text;
        var password = PasswordBox.Password;

        try
        {
            DatabaseContext.LogIn(login, password);

            var usersManager = new UsersManager();
            User = await usersManager.GetUserAsync(login);

            MessageBox.Show(this, $"Authenticated as {User.Role.ToString()}/{User.Login}", "Info", MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Error while authenticating: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void ChangePassword(object sender, RoutedEventArgs e)
    {
        var login = LoginBox.Text;
        var password = PasswordBox.Password;
        var newPassword = NewPasswordBox.Password;

        try
        {
            DatabaseContext.LogIn(login, password);
            var usersManager = new UsersManager();
            await usersManager.ChangePasswordAsync(login, newPassword, password);

            MessageBox.Show(this, "Password changed. Please authenticate with new password", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            DatabaseContext.LogOff();

            DialogResult = true;
            User = null;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Error while authenticating: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}