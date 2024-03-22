using DatabaseClient.Users;

namespace GuiClient.Extensions;

public static class UserExtensions
{
    public static bool IsOwner(this User user) => user?.Role >= Role.Owner;

    public static bool IsAdmin(this User user) => user?.Role >= Role.Admin;

    public static bool IsWorker(this User user) => user?.Role >= Role.Worker;
}