using DatabaseClient.Principals;

namespace DatabaseClient.Providers;

public interface IPrincipalProvider
{
    Principal GetPrincipal();
}