using System.Net;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Contexts;

public class DatabaseContextFactory(NetworkCredential credential)
{
    public DatabaseContext Create()
    {
        var connectionString = new SqlConnectionStringBuilder
        {
            DataSource = "SHAPOVAL-M-NB\\SQLEXPRESS",
            InitialCatalog = "bsbd_kursach",
            UserID = credential.UserName,
            Password = credential.Password,
            Encrypt = true
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