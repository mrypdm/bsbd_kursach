using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AuthWindowViewModel(ISecurityContext securityContext, bool changePassword) : BaseViewModel
{
    public Visibility ChangePasswordModeVisibility => changePassword ? Visibility.Visible : Visibility.Collapsed;

    public Visibility LoginModeVisibility => !changePassword ? Visibility.Visible : Visibility.Collapsed;

    public ICommand ChangePassword => new AsyncFuncCommand<AuthWindow>(ChangePasswordInternal);

    public ICommand LogIn => new AsyncFuncCommand<AuthWindow>(LogInInternal);

    private async Task ChangePasswordInternal(AuthWindow window)
    {
        try
        {
            using var password = window.PasswordBox.SecurePassword;
            using var newPassword = window.NewPasswordBox.SecurePassword;

            await securityContext.ChangePasswordAsync(password, newPassword);

            MessageBox.Show(window, "Password changed. Please authenticate with new password", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            window.DialogResult = true;
            window.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(window, ex.Message, "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async Task LogInInternal(AuthWindow window)
    {
        try
        {
            var username = window.UserNameBox.Text;
            using var password = window.PasswordBox.SecurePassword;

            await securityContext.LogInAsync(username, password);

            MessageBox.Show(window, $"Authenticated as {securityContext.User}", "Info",
                MessageBoxButton.OK, MessageBoxImage.Information);

            window.DialogResult = true;
            window.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(window, ex.Message, "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}