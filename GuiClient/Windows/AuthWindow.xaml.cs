using System;
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
    {
        DatabaseContext.LogOff();
        InitializeComponent();
    }

    public User User { get; private set; }

    private async void AuthButton_Click(object sender, RoutedEventArgs e)
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
}