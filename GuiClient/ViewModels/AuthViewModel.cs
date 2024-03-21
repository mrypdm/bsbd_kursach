using System;
using System.Windows.Input;
using DatabaseClient.Users;
using Domain;
using GuiClient.Commands;
using GuiClient.UserControls;
using GuiClient.Windows;

namespace GuiClient.ViewModels;

public class AuthViewModel(AuthUserControl control) : BaseViewModel<AuthUserControl>(control)
{
    // For XAML
    public AuthViewModel()
        : this(null)
    {
    }

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

    public string UserText => CurrentUser == null ? string.Empty : $"{CurrentUser.Role}/{CurrentUser.Login}";

    public ICommand Authenticate => new Command(AuthenticateInternal);

    public ICommand ChangePassword => new Command(ChangePasswordInternal);

    private void AuthenticateInternal()
    {
        if (CurrentUser != null)
        {
            LogOff();
            return;
        }

        var authWindow = new AuthWindow();
        if (authWindow.ShowDialog() != true)
        {
            return;
        }

        LogIn(authWindow.User);
    }

    private void ChangePasswordInternal()
    {
        if (CurrentUser == null)
        {
            Logging.Logger.Error("Something wrong. User can change password while unauthorized");
            throw new InvalidOperationException("Attempt to change password for null user");
        }

        var authWindow = new AuthWindow(true, CurrentUser);
        if (authWindow.ShowDialog() != true)
        {
            return;
        }

        LogOff();
    }

    private void LogIn(User user)
    {
        CurrentUser = user;
        ChangePasswordButtonEnabled = true;
    }

    private void LogOff()
    {
        AuthWindow.LogOff();
        CurrentUser = null;
        ChangePasswordButtonEnabled = false;
    }
}