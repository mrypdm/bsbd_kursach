using System.Net;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Users;

namespace GuiClient;

public class SecurityContext : NotifyPropertyChanged
{
    private string _username;
    private Role _role;

    private SecurityContext()
    {
    }

    public static void Init()
    {
        Instance = new SecurityContext();
    }

    public static string UserName
    {
        get => Instance._username;
        private set => Instance.SetField(ref Instance._username, value);
    }

    public static Role Role
    {
        get => Instance._role;
        private set => Instance.SetField(ref Instance._role, value);
    }

    public static bool IsAuthenticated => UserName is not null;

    public static SecurityContext Instance { get; private set; }

    public static async Task LogInAsync(string username, string password)
    {
        var factory = new DatabaseContextFactory(new NetworkCredential(username, password));
        var usersManager = new UsersManager(factory);

        Role = await usersManager.GetUserRoleAsync(username);
        UserName = username;
    }

    public static void LogOff()
    {
        Role = default;
        UserName = default;
    }

    public static async Task ChangePasswordAsync(string username, string password, string newPassword)
    {
        var factory = new DatabaseContextFactory(new NetworkCredential(username, password));
        var usersManager = new UsersManager(factory);

        await usersManager.ChangePasswordAsync(username, password, newPassword);

        LogOff();
    }
}