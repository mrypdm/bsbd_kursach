using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DatabaseClient.Contexts;
using DatabaseClient.Users;
using GuiClient.Commands;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AuthWindowViewModel : WindowViewModel<AuthWindow>
{
    public AuthWindowViewModel(bool changePassword)
        : base(new AuthWindow())
    {
        ChangePasswordModeVisibility = changePassword ? Visibility.Visible : Visibility.Collapsed;
        LoginModeVisibility = !changePassword ? Visibility.Visible : Visibility.Collapsed;
        IsUserNameEnabled = !changePassword;

        Control.InitializeComponent();

        Control.UserNameBox.Text = changePassword ? SecurityContext.UserName : string.Empty;
    }

    public bool IsUserNameEnabled { get; }

    public Visibility ChangePasswordModeVisibility { get; }

    public Visibility LoginModeVisibility { get; }

    public ICommand ChangePassword => new AsyncCommand(ChangePasswordInternal);

    public ICommand LogIn => new AsyncCommand(LogInInternal);

    private async Task ChangePasswordInternal()
    {
        try
        {
            var username = Control.UserNameBox.Text;
            var password = Control.PasswordBox.Password;
            var newPassword = Control.NewPasswordBox.Password;

            await SecurityContext.ChangePasswordAsync(username, password, newPassword);

            MessageBox.Show(Control, "Password changed. Please authenticate with new password", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

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

            await SecurityContext.LogInAsync(username, password);

            MessageBox.Show(Control,
                $"Authenticated as {SecurityContext.Role}/{SecurityContext.UserName}", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

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