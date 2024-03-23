using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        Control.UserNameBox.Text = changePassword ? SecurityContext.Instance.User.UserName : string.Empty;
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
            using var password = Control.PasswordBox.SecurePassword;
            using var newPassword = Control.NewPasswordBox.SecurePassword;

            await SecurityContext.Instance.ChangePasswordAsync(username, password, newPassword);

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
            using var password = Control.PasswordBox.SecurePassword;

            await SecurityContext.Instance.LogInAsync(username, password);

            MessageBox.Show(Control,
                $"Authenticated as {SecurityContext.Instance.User}", "Info",
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