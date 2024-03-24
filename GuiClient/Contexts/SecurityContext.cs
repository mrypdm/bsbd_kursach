using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Options;
using DatabaseClient.Principals;
using DatabaseClient.Providers;

namespace GuiClient.Contexts;

public sealed class SecurityContext(ServerOptions options) : ISecurityContext
{
    private Principal _principal;

    public bool IsAuthenticated => Principal is not null;

    public Principal Principal
    {
        get => _principal;
        private set
        {
            SetField(ref _principal, value);
            OnPropertyChanged(nameof(IsAuthenticated));
        }
    }

    public async Task LogInAsync(string userName, SecureString password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(password);

        var factory = new DatabaseContextFactory(new Principal(userName, password, default), options);
        var usersManager = new PrincipalsManager(factory);

        var principal = await usersManager.GetPrincipalByName(userName);

        LogOff();
        Principal = new Principal(principal.Name, password, principal.Role);
    }

    public async Task ChangePasswordAsync(SecureString oldPassword, SecureString newPassword)
    {
        if (!IsAuthenticated)
        {
            throw new InvalidOperationException("User is not authenticated");
        }

        ArgumentNullException.ThrowIfNull(oldPassword);
        ArgumentNullException.ThrowIfNull(newPassword);

        using var oldCred = new Principal(Principal.Name, oldPassword.Copy(), default);

        var factory = new DatabaseContextFactory(oldCred, options);
        var usersManager = new PrincipalsManager(factory);

        await usersManager.ChangePasswordAsync(oldCred.Name, oldCred.SecurePassword, newPassword);

        LogOff();
    }

    public void LogOff()
    {
        Principal?.Dispose();
        Principal = default;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    Principal IPrincipalProvider.GetPrincipal()
    {
        return Principal;
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }
}