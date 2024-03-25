using DatabaseClient.Models;

namespace DatabaseClient.Extensions;

public static class PrincipalExtensions
{
    public static bool IsOwner(this DbPrincipal principal)
    {
        return principal?.Role <= Role.Owner;
    }

    public static bool IsAdmin(this DbPrincipal principal)
    {
        return principal?.Role <= Role.Admin;
    }

    public static bool IsWorker(this DbPrincipal principal)
    {
        return principal?.Role <= Role.Worker;
    }
}