using System.ComponentModel;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Providers;
using DatabaseClient.Users;

namespace GuiClient.Contexts;

public interface ISecurityContext : ICredentialProvider, INotifyPropertyChanged
{
    bool IsAuthenticated { get; }

    User User { get; }

    Task LogInAsync(string userName, SecureString password);

    Task ChangePasswordAsync(SecureString oldPassword, SecureString newPassword);

    void LogOff();
}