using DatabaseClient.Options;
using DatabaseClient.Providers;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Contexts;

public class DatabaseContextFactory(ICredentialProvider credentialProvider, ServerOptions options)
{
    public DatabaseContext Create()
    {
        var credential = credentialProvider.Credential;

        var connectionString = new SqlConnectionStringBuilder
        {
            DataSource = options.ServerName,
            InitialCatalog = options.DatabaseName,
            UserID = credential.UserName,
            Password = credential.Password,
            Encrypt = options.Encryption
        }.ConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connectionString);

        if (Logging.IsInit)
        {
            optionsBuilder = optionsBuilder.UseLoggerFactory(Logging.LoggerFactory);
        }

        return new DatabaseContext(optionsBuilder.Options);
    }
}