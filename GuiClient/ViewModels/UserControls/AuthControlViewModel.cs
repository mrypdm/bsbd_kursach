using System;
using System.Windows.Input;
using Domain;
using GuiClient.Commands;
using GuiClient.ViewModels.Windows;
using GuiClient.Views.UserControls;

namespace GuiClient.ViewModels.UserControls;

public class AuthControlViewModel : BaseViewModel<AuthUserControl>
{
    public AuthControlViewModel(AuthUserControl control)
        : base(control)
    {
        SecurityContext.Instance.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(ChangePasswordButtonEnabled));
            OnPropertyChanged(nameof(AuthButtonText));
            OnPropertyChanged(nameof(UserText));
        };
    }

    public bool ChangePasswordButtonEnabled => SecurityContext.IsAuthenticated;

    public string AuthButtonText => SecurityContext.IsAuthenticated ? "Log Off" : "Log In";

    public string UserText => SecurityContext.IsAuthenticated
        ? $"{SecurityContext.Role}/{SecurityContext.UserName}"
        : string.Empty;

    public ICommand Authenticate => new Command(AuthenticateInternal);

    public ICommand ChangePassword => new Command(ChangePasswordInternal);

    private void AuthenticateInternal()
    {
        if (SecurityContext.IsAuthenticated)
        {
            SecurityContext.LogOff();
            return;
        }

        var authWindowViewModel = new AuthWindowViewModel(false);
        authWindowViewModel.ShowDialog();
    }

    private void ChangePasswordInternal()
    {
        if (!SecurityContext.IsAuthenticated)
        {
            Logging.Logger.Error("Something wrong. User can change password while unauthenticated");
            throw new InvalidOperationException("Attempt to change password for null user");
        }

        var authWindowViewModel = new AuthWindowViewModel(true);
        authWindowViewModel.ShowDialog();
    }
}