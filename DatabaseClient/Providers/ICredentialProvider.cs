using System.Net;

namespace DatabaseClient.Providers;

public interface ICredentialProvider
{
    NetworkCredential Credential { get; }
}