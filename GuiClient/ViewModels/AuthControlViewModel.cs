using System;
using System.Windows.Input;
using DatabaseClient.Users;
using Domain;
using GuiClient.Commands;
using GuiClient.UserControls;

namespace GuiClient.ViewModels;

public class AuthControlViewModel(AuthUserControl control) : BaseViewModel<AuthUserControl>(control)
{
    private bool _isChangePasswordEnabled;
    private User _currentUser;

    public User CurrentUser
    {
        get => _currentUser;
        set
        {
            SetField(ref _currentUser, value);
            OnPropertyChanged(nameof(UserText));
            OnPropertyChanged(nameof(AuthButtonText));
        }
    }

    public bool ChangePasswordButtonEnabled
    {
        get => _isChangePasswordEnabled;
        set => SetField(ref _isChangePasswordEnabled, value);
    }

    public string AuthButtonText => CurrentUser == null ? "Log In" : "Log Off";

    public string UserText => CurrentUser == null ? string.Empty : $"{CurrentUser.Role}/{CurrentUser.UserName}";

    public ICommand Authenticate => new Command(AuthenticateInternal);

    public ICommand ChangePassword => new Command(ChangePasswordInternal);

    private void AuthenticateInternal()
    {
        if (CurrentUser != null)
        {
            LogOff();
            return;
        }

        var authWindowViewModel = new AuthWindowViewModel();
        if (authWindowViewModel.ShowDialog() != true)
        {
            return;
        }

        CurrentUser = authWindowViewModel.User;
        ChangePasswordButtonEnabled = true;
    }

    private void ChangePasswordInternal()
    {
        if (CurrentUser == null)
        {
            Logging.Logger.Error("Something wrong. User can change password while unauthorized");
            throw new InvalidOperationException("Attempt to change password for null user");
        }

        var authWindowViewModel = new AuthWindowViewModel(CurrentUser);
        if (authWindowViewModel.ShowDialog() != true)
        {
            return;
        }

        LogOff();
    }

    private void LogOff()
    {
        AuthWindowViewModel.LogOff();
        CurrentUser = null;
        ChangePasswordButtonEnabled = false;
    }
}