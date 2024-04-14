using DatabaseClient.Models;

namespace DatabaseClient.Providers;

public interface IPrincipalProvider
{
    Principal GetPrincipal();
}