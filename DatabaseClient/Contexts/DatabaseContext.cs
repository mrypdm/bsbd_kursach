using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Contexts;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    // Tables

    public DbSet<Book> Books { get; set; }

    public DbSet<Client> Clients { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrdersToBook> OrdersToBooks { get; set; }

    public DbSet<Review> Reviews { get; set; }

    public DbSet<Tag> Tags { get; set; }

    // Views

    public DbSet<DbPrincipal> Principals { get; set; }

    // Database configuration

    protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("bsbd_mark_book_as_deleted"));

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
            entity.ToTable(tb => tb.HasTrigger("bsbd_mark_user_deleted"));

            entity.HasIndex(e => e.Phone, "IX_Clients_Phone")
                .IsUnique()
                .HasFilter("([Phone]<>'0000000000')");

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<DbPrincipal>(entity =>
        {
            entity.HasNoKey().ToView("bsbd_principals");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(128)
                .UseCollation("Latin1_General_100_CI_AS_KS_WS_SC");
            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(128)
                .UseCollation("Latin1_General_100_CI_AS_KS_WS_SC");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("bsbd_prevent_update_orders"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Clients");
        });

        modelBuilder.Entity<OrdersToBook>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.BookId });

            entity.ToTable(tb =>
            {
                tb.HasTrigger("bsbd_delete_order");
                tb.HasTrigger("bsbd_verify_order");
            });

            entity.HasOne(d => d.Book).WithMany(p => p.OrdersToBooks)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
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
            entity.HasIndex(e => e.Name, "IX_Tags_Title").IsUnique();

            entity.Property(e => e.Name)
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