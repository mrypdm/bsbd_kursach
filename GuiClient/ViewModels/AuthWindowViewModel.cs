using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DatabaseClient.Contexts;
using DatabaseClient.Users;
using GuiClient.Commands;
using GuiClient.Windows;

namespace GuiClient.ViewModels;

public class AuthWindowViewModel : WindowViewModel<AuthWindow>
{
    public AuthWindowViewModel(User user = null)
        : base(new AuthWindow())
    {
        ChangePasswordModeVisibility = user != null ? Visibility.Visible : Visibility.Collapsed;
        LoginModeVisibility = user == null ? Visibility.Visible : Visibility.Collapsed;
        User = user;

        Control.InitializeComponent();

        Control.UserNameBox.Text = user?.UserName ?? string.Empty;
    }

    public User User { get; private set; }

    public bool IsUserNameEnabled => User == null;

    public Visibility ChangePasswordModeVisibility { get; }

    public Visibility LoginModeVisibility { get; }

    public ICommand ChangePassword => new AsyncCommand(ChangePasswordInternal);

    public ICommand LogIn => new AsyncCommand(LogInInternal);

    public static void LogOff() => DatabaseContext.LogOff();

    private async Task ChangePasswordInternal()
    {
        try
        {
            var username = Control.UserNameBox.Text;
            var password = Control.PasswordBox.Password;
            var newPassword = Control.NewPasswordBox.Password;

            DatabaseContext.LogIn(username, password);

            var usersManager = new UsersManager();
            await usersManager.ChangePasswordAsync(username, newPassword, password);

            MessageBox.Show(Control, "Password changed. Please authenticate with new password", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            DatabaseContext.LogOff();

            Control.DialogResult = true;
            Control.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(Control, $"Error while authenticating: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async Task LogInInternal()
    {
        try
        {
            var username = Control.UserNameBox.Text;
            var password = Control.PasswordBox.Password;

            DatabaseContext.LogIn(username, password);

            var usersManager = new UsersManager();
            var role = await usersManager.GetUserRoleAsync(username);

            MessageBox.Show(Control, $"Authenticated as {role}/{username}", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            User = new User(username, password, role);
            Control.DialogResult = true;
            Control.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(Control, $"Error while authenticating: {ex.Message}", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}