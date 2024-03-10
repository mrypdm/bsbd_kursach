using System;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Contexts;

public static class DatabaseContextFactory
{
    private static string _user;

    private static string _password;

    private static DatabaseContext _context;

    private static string GetConnectionString() =>
        $"Server=SHAPOVAL-M-NB\\SQLEXPRESS;" +
        $"Database=bsbd_kursach;" +
        $"User Id={_user};" +
        $"Password={_password};" +
        $"TrustServerCertificate=True;";

    /// <summary>
    /// Current context
    /// </summary>
    /// <exception cref="InvalidOperationException">If user is not authenticated</exception>
    public static DatabaseContext Context => _context ?? throw new InvalidOperationException("User unauthorized");

    /// <summary>
    /// Creates context for user
    /// </summary>
    public static void LogIn(string user, string password)
    {
        if (_user != user || _password != password)
        {
            LogOff();
        }

        _user = user;
        _password = password;

        var connectionString = GetConnectionString();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connectionString)
            .Options;

        _context = new DatabaseContext(options);
    }

    /// <summary>
    /// Disposes current context
    /// </summary>
    public static void LogOff()
    {
        _context?.Dispose();
        _context = null;
    }
}