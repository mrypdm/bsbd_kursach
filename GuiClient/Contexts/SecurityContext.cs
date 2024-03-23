using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Providers;
using DatabaseClient.Users;

namespace GuiClient.Contexts;

public sealed class SecurityContext : ISecurityContext
{
    private User _user;

    public bool IsAuthenticated => User is not null;

    public User User
    {
        get => _user;
        private set
        {
            SetField(ref _user, value);
            OnPropertyChanged(nameof(Credential));
            OnPropertyChanged(nameof(IsAuthenticated));
        }
    }

    public NetworkCredential Credential => User?.Credential;

    public async Task LogInAsync(string userName, SecureString password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(password);

        var cred = new NetworkCredential(userName, password);

        var factory = new DatabaseContextFactory(new CredentialProvider(cred));
        var usersManager = new UsersManager(factory);

        var role = await usersManager.GetUserRoleAsync(userName);

        LogOff();
        User = new User(userName, password, role);
    }

    public async Task ChangePasswordAsync(SecureString oldPassword, SecureString newPassword)
    {
        if (!IsAuthenticated)
        {
            throw new InvalidOperationException("User is not authenticated");
        }

        ArgumentNullException.ThrowIfNull(oldPassword);
        ArgumentNullException.ThrowIfNull(newPassword);

        var oldCred = new NetworkCredential(User.UserName, oldPassword);
        var newCred = new NetworkCredential(User.UserName, newPassword);

        var factory = new DatabaseContextFactory(new CredentialProvider(oldCred));
        var usersManager = new UsersManager(factory);

        await usersManager.ChangePasswordAsync(User.UserName, oldCred.Password, newCred.Password);

        LogOff();
    }

    public void LogOff()
    {
        User?.Dispose();
        User = default;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}