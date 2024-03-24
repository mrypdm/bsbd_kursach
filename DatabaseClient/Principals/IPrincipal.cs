namespace DatabaseClient.Principals;

public interface IPrincipal
{
    string Name { get; }

    Role Role { get; }
}