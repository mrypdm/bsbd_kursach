using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace GuiClient.ViewModels.UserControls;

public class AuthControlViewModel : AuthenticatedViewModel
{
    public AuthControlViewModel(ISecurityContext securityContext)
        : base(securityContext)
    {
        Authenticate = new AsyncActionCommand(AuthenticateInternal);
        ChangePassword = new AsyncActionCommand(ChangePasswordInternal);

        SecurityContext.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(ChangePasswordButtonEnabled));
            OnPropertyChanged(nameof(AuthButtonText));
            OnPropertyChanged(nameof(UserText));
        };
    }

    public bool ChangePasswordButtonEnabled => SecurityContext.IsAuthenticated;

    public string AuthButtonText => SecurityContext.IsAuthenticated ? "Log Off" : "Log In";

    public string UserText => SecurityContext.IsAuthenticated
        ? SecurityContext.Principal.ToString()
        : string.Empty;

    public ICommand Authenticate { get; }

    public ICommand ChangePassword { get; }

    private async Task AuthenticateInternal()
    {
        if (SecurityContext.IsAuthenticated)
        {
            SecurityContext.LogOff();
            return;
        }

        if (!CredAsker.AskForLogin(out var userName, out var password))
        {
            return;
        }

        using (password)
        {
            await SecurityContext.LogInAsync(userName, password);
        }
    }

    private async Task ChangePasswordInternal()
    {
        if (!SecurityContext.IsAuthenticated)
        {
            throw new InvalidOperationException("Attempt to change password for null user");
        }

        if (!CredAsker.AskForChangePassword(SecurityContext.Principal.Name, out var password, out var newPassword))
        {
            return;
        }

        using (password)
        using (newPassword)
        {
            await SecurityContext.ChangePasswordAsync(password, newPassword);
            MessageBox.Show("Password changed. Please authenticate with new password", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}