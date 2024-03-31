using DatabaseClient.Models;

namespace DatabaseClient.Extensions;

public static class PrincipalExtensions
{
    public static bool IsSecurity(this Principal principal)
    {
        return principal?.Role <= Role.Security;
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