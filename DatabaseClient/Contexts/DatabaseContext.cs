using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DatabaseClient.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog.Extensions.Logging;

namespace DatabaseClient.Contexts;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    private static string _user;

    private static string _password;

    private static DatabaseContext _context;

    private static SerilogLoggerFactory _loggerFactory;

    /// <summary>
    /// Current context
    /// </summary>
    /// <exception cref="InvalidOperationException">If user is not authenticated</exception>
    public static DatabaseContext Instance => _context ?? throw new InvalidOperationException("User unauthorized");

    // Tables

    public DbSet<Book> Books { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrdersToBook> OrdersToBooks { get; set; }

    public DbSet<Review> Reviews { get; set; }

    public DbSet<Tag> Tags { get; set; }

    private static string GetConnectionString()
    {
        return new SqlConnectionStringBuilder
        {
            DataSource = "SHAPOVAL-M-NB\\SQLEXPRESS",
            InitialCatalog = "bsbd_kursach",
            UserID = _user,
            Password = _password,
            TrustServerCertificate = true
        }.ConnectionString;
    }

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

        _loggerFactory = new SerilogLoggerFactory();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connectionString)
            .UseLoggerFactory(_loggerFactory)
            .Options;

        _context = new DatabaseContext(options);
    }

    /// <summary>
    /// Disposes current context
    /// </summary>
    public static void LogOff()
    {
        _context?.Dispose();
        _loggerFactory?.Dispose();
        _context = null;
        _loggerFactory = null;
    }

    // Database configuration

    protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(e => e.Author, "IX_Books_Author");

            entity.HasIndex(e => e.Title, "IX_Books_Title");

            entity.Property(e => e.Author)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasIndex(e => e.Phone, "IX_Clients_Phone").IsUnique();

            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Orders_Clients");
        });

        modelBuilder.Entity<OrdersToBook>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.BookId });

            entity.HasOne(d => d.Book).WithMany(p => p.OrdersToBooks)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_OrdersToBooks_Books");

            entity.HasOne(d => d.Order).WithMany(p => p.OrdersToBooks)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrdersToBooks_Orders");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_Review_Books");

            entity.HasOne(d => d.Client).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("FK_Review_Clients");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(e => e.Title, "IX_Tags_Title").IsUnique();

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasMany(d => d.Books).WithMany(p => p.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "BooksToTag",
                    r => r.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .HasConstraintName("FK_BooksToTags_Books"),
                    l => l.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_BooksToTags_Tags"),
                    j =>
                    {
                        j.HasKey("TagId", "BookId");
                        j.ToTable("BooksToTags");
                    });
        });

        base.OnModelCreating(modelBuilder);
    }
}