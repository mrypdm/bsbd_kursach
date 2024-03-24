using System;
using System.Windows.Input;
using Domain;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.ViewModels.Windows;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class AuthControlViewModel : AuthenticatedViewModel
{
    public AuthControlViewModel(ISecurityContext securityContext)
        : base(securityContext)
    {
        Authenticate = new ActionCommand(AuthenticateInternal);
        ChangePassword = new ActionCommand(ChangePasswordInternal);

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

    private void AuthenticateInternal()
    {
        if (SecurityContext.IsAuthenticated)
        {
            SecurityContext.LogOff();
            return;
        }

        var viewModel = new AuthWindowViewModel(SecurityContext, false);
        var window = new AuthWindow(viewModel);
        window.ShowDialog();
    }

    private void ChangePasswordInternal()
    {
        if (!SecurityContext.IsAuthenticated)
        {
            Logging.Logger.Error("Something wrong. User can change password while unauthenticated");
            throw new InvalidOperationException("Attempt to change password for null user");
        }

        var viewModel = new AuthWindowViewModel(SecurityContext, true);
        var window = new AuthWindow(viewModel);
        window.ShowDialog();
    }
}