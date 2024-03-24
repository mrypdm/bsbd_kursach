using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
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
            OnPropertyChanged(nameof(Credential));
            OnPropertyChanged(nameof(IsAuthenticated));
        }
    }

    public NetworkCredential Credential => Principal?.Credential;

    public async Task LogInAsync(string userName, SecureString password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentNullException.ThrowIfNull(password);

        var cred = new NetworkCredential(userName, password);

        var factory = new DatabaseContextFactory(new CredentialProvider(cred), options);
        var usersManager = new PrincipalsManager(factory);

        var role = await usersManager.GetPrincipalRoleAsync(userName);

        LogOff();
        Principal = new Principal(userName, password, role);
    }

    public async Task ChangePasswordAsync(SecureString oldPassword, SecureString newPassword)
    {
        if (!IsAuthenticated)
        {
            throw new InvalidOperationException("User is not authenticated");
        }

        ArgumentNullException.ThrowIfNull(oldPassword);
        ArgumentNullException.ThrowIfNull(newPassword);

        var oldCred = new NetworkCredential(Principal.Name, oldPassword);
        var newCred = new NetworkCredential(Principal.Name, newPassword);

        var factory = new DatabaseContextFactory(new CredentialProvider(oldCred), options);
        var usersManager = new PrincipalsManager(factory);

        await usersManager.ChangePasswordAsync(Principal.Name, oldCred.Password, newCred.Password);

        LogOff();
    }

    public void LogOff()
    {
        Principal?.Dispose();
        Principal = default;
    }

    public event PropertyChangedEventHandler PropertyChanged;

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