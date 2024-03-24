using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AuthWindowViewModel : BaseViewModel
{
    private readonly ISecurityContext _securityContext;

    public AuthWindowViewModel(ISecurityContext securityContext, bool changePassword)
    {
        _securityContext = securityContext;
        ChangePasswordModeVisibility = changePassword ? Visibility.Visible : Visibility.Collapsed;
        LoginModeVisibility = !changePassword ? Visibility.Visible : Visibility.Collapsed;

        ChangePassword = new AsyncFuncCommand<AuthWindow>(ChangePasswordInternal);
        LogIn = new AsyncFuncCommand<AuthWindow>(LogInInternal);
    }

    public Visibility ChangePasswordModeVisibility { get; }

    public Visibility LoginModeVisibility { get; }

    public ICommand ChangePassword { get; }

    public ICommand LogIn { get; }

    private async Task ChangePasswordInternal(AuthWindow window)
    {
        try
        {
            using var password = window.PasswordBox.SecurePassword;
            using var newPassword = window.NewPasswordBox.SecurePassword;

            await _securityContext.ChangePasswordAsync(password, newPassword);

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

            await _securityContext.LogInAsync(username, password);

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