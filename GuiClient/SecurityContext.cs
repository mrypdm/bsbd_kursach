using System;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Users;

namespace GuiClient;

public class SecurityContext : NotifyPropertyChanged
{
    private User _user;

    private SecurityContext()
    {
    }

    public static void Init()
    {
        Instance = new SecurityContext();
    }

    public static bool IsAuthenticated => Instance.User is not null;

    public static SecurityContext Instance { get; private set; }

    public User User
    {
        get => _user;
        private set
        {
            SetField(ref _user, value);
            OnPropertyChanged(nameof(IsAuthenticated));
        }
    }

    public async Task LogInAsync(string username, SecureString password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentNullException.ThrowIfNull(password);

        var cred = new NetworkCredential(username, password);
        var factory = new DatabaseContextFactory(cred);
        var usersManager = new UsersManager(factory);

        var role = await usersManager.GetUserRoleAsync(username);

        User = new User(username, password, role);
    }

    public void LogOff()
    {
        User = default;
    }

    public async Task ChangePasswordAsync(string username, SecureString password, SecureString newPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(newPassword);

        var oldCred = new NetworkCredential(username, password);
        var newCred = new NetworkCredential(username, newPassword);

        var factory = new DatabaseContextFactory(oldCred);
        var usersManager = new UsersManager(factory);

        await usersManager.ChangePasswordAsync(username, oldCred.Password, newCred.Password);

        LogOff();
    }
}