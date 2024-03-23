using System.Net;

namespace DatabaseClient.Providers;

public class CredentialProvider(NetworkCredential credential) : ICredentialProvider
{
    public NetworkCredential Credential => credential;
}