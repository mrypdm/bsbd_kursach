using DatabaseClient.Users;

namespace GuiClient.Extensions;

public static class UserExtensions
{
    public static bool IsOwner(this User user)
    {
        return user?.Role >= Role.Owner;
    }

    public static bool IsAdmin(this User user)
    {
        return user?.Role >= Role.Admin;
    }

    public static bool IsWorker(this User user)
    {
        return user?.Role >= Role.Worker;
    }
}