using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Options;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;

namespace GuiClient.Contexts;

public sealed class SecurityContext(ServerOptions options) : ISecurityContext
{
    private DbPrincipal _principal;

    public bool IsAuthenticated => Principal is not null;

    public DbPrincipal Principal
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

        using var cred = new DbPrincipal();
        cred.Name = userName;
        cred.SecurePassword = password.Copy();

        var factory = new DatabaseContextFactory(cred, options);
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

        using var oldCred = new DbPrincipal();
        oldCred.Name = Principal.Name;
        oldCred.SecurePassword = oldPassword.Copy();

        var factory = new DatabaseContextFactory(oldCred, options);
        var repository = new PrincipalRepository(factory);

        await repository.ChangePasswordAsync(oldCred, newPassword);

        LogOff();
    }

    public void LogOff()
    {
        Principal?.Dispose();
        Principal = default;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    DbPrincipal IPrincipalProvider.GetPrincipal()
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