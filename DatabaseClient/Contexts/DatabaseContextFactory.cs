using DatabaseClient.Options;
using DatabaseClient.Providers;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Contexts;

public class DatabaseContextFactory(IPrincipalProvider principalProvider, ServerOptions options)
{
    public DatabaseContext Create()
    {
        var credential = principalProvider.GetPrincipal();

        var connectionString = new SqlConnectionStringBuilder
        {
            DataSource = options.ServerName,
            InitialCatalog = options.DatabaseName,
            UserID = credential.Name,
            Password = credential.Password,
            Encrypt = options.Encryption
        }.ConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connectionString);

        if (Logging.IsInit)
        {
            optionsBuilder = optionsBuilder
                .UseLoggerFactory(Logging.LoggerFactory)
#if DEBUG
                .EnableSensitiveDataLogging();
#endif
        }

        return new DatabaseContext(optionsBuilder.Options);
    }
}