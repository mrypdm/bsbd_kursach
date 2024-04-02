using System;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Options;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;

namespace GuiClient.Contexts;

public sealed class SecurityContext(ServerOptions options) : NotifyPropertyChanged, ISecurityContext
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

        using var cred = new Principal();
        cred.Name = userName;
        cred.SecurePassword = password.Copy();

        var factory = new DbContextFactory(cred, options);
        var repository = new PrincipalRepository(factory);

        LogOff();

        Principal = await repository.GetByName(userName);
        Principal.SecurePassword = password.Copy();
    }

    public async Task ChangePasswordAsync(SecureString oldPassword, SecureString newPassword)
    {
        if (!IsAuthenticated)
        {
            throw new InvalidOperationException("User is not authenticated");
        }

        ArgumentNullException.ThrowIfNull(oldPassword);
        ArgumentNullException.ThrowIfNull(newPassword);

        using var oldCred = new Principal();
        oldCred.Name = Principal.Name;
        oldCred.SecurePassword = oldPassword.Copy();

        var factory = new DbContextFactory(oldCred, options);
        var repository = new PrincipalRepository(factory);

        await repository.ChangePasswordAsync(oldCred, newPassword);

        LogOff();
    }

    public void LogOff()
    {
        Principal?.Dispose();
        Principal = default;
    }

    Principal IPrincipalProvider.GetPrincipal()
    {
        return Principal;
    }
}