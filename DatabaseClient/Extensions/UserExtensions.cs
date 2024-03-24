using DatabaseClient.Principals;

namespace DatabaseClient.Extensions;

public static class UserExtensions
{
    public static bool IsOwner(this Principal principal)
    {
        return principal?.Role <= Role.Owner;
    }

    public static bool IsAdmin(this Principal principal)
    {
        return principal?.Role <= Role.Admin;
    }

    public static bool IsWorker(this Principal principal)
    {
        return principal?.Role <= Role.Worker;
    }
}