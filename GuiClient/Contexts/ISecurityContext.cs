using System.ComponentModel;
using System.Security;
using System.Threading.Tasks;
using DatabaseClient.Principals;
using DatabaseClient.Providers;

namespace GuiClient.Contexts;

public interface ISecurityContext : ICredentialProvider, INotifyPropertyChanged
{
    bool IsAuthenticated { get; }

    Principal Principal { get; }

    Task LogInAsync(string userName, SecureString password);

    Task ChangePasswordAsync(SecureString oldPassword, SecureString newPassword);

    void LogOff();
}