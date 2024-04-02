using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Options;
using DatabaseClient.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ConsoleClient;

public static class Startup
{
    public static Principal Cred { get; } = new()
    {
        Name = "bsbd_owner",
        SecurePassword = "very_secret_Password_forOwner".AsSecure()
    };

    public static async Task InitDatabaseAsync()
    {
        var factory = new DatabaseContextFactory(Cred, new ServerOptions());

        var clientsRepository = new ClientsRepository(factory);
        var tagsRepository = new TagsRepository(factory);
        var booksRepository = new BooksRepository(factory);
        var reviewsRepository = new ReviewsRepository(factory);
        var ordersRepository = new OrdersRepository(factory);

        var cl1 = await clientsRepository.AddClientAsync("FName1", "LName1", "0987654321", Gender.Male);
        var cl2 = await clientsRepository.AddClientAsync("FName2", "LName2", "8005553535", Gender.Male);
        var cl3 = await clientsRepository.AddClientAsync("FName3", "LName3", "7777777777", Gender.Male);
        var cl4 = await clientsRepository.AddClientAsync("FName4", "LName4", "0000000000", Gender.Male);
        var cl5 = await clientsRepository.AddClientAsync("FName5", "LName5", "1234567890", Gender.Female);

        var tag1 = await tagsRepository.AddTagAsync("action");
        var tag2 = await tagsRepository.AddTagAsync("fanfiction");
        var tag3 = await tagsRepository.AddTagAsync("history");
        var tag4 = await tagsRepository.AddTagAsync("romance");
        var tag5 = await tagsRepository.AddTagAsync("science");

        var b1 = await booksRepository.AddBookAsync("Very interesting book", "Famous Author", DateTime.Now, 1000, 123);
        var b2 = await booksRepository.AddBookAsync("Very interesting book 2", "Famous Author", DateTime.Now.AddDays(3),
            1300, 91);
        var b3 = await booksRepository.AddBookAsync("Pretty boring book", "Silly Author", DateTime.Now.AddDays(1), 300,
            34);
        var b4 = await booksRepository.AddBookAsync("Rocket Science", "Scamer", DateTime.Now.AddHours(9), 550, 12);

        await booksRepository.AddTagToBookAsync(b1, tag1);
        await booksRepository.AddTagToBookAsync(b1, tag2);
        await booksRepository.AddTagToBookAsync(b2, tag1);
        await booksRepository.AddTagToBookAsync(b2, tag2);
        await booksRepository.AddTagToBookAsync(b3, tag3);
        await booksRepository.AddTagToBookAsync(b3, tag4);
        await booksRepository.AddTagToBookAsync(b4, tag5);

        foreach (var cl in new[] { cl1, cl2, cl3, cl4, cl5 })
        {
            foreach (var b in new[] { b1, b2, b3, b4 })
            {
                await reviewsRepository.AddReviewAsync(cl, b, Random.Shared.Next(1, 6));
            }
        }

        await ordersRepository.AddOrderAsync(cl1, new[]
        {
            new OrdersToBook
            {
                BookId = b1.Id,
                Count = 1
            },
            new OrdersToBook
            {
                BookId = b2.Id,
                Count = 1
            }
        });
        await ordersRepository.AddOrderAsync(cl2, new[]
        {
            new OrdersToBook
            {
                BookId = b4.Id,
                Count = 2
            }
        });
    }

    public static async Task ClearAllAsync()
    {
        var factory = new DatabaseContextFactory(Cred, new ServerOptions());
        await using var context = factory.Create();

        await context.Reviews.ExecuteDeleteAsync();
        await context.Orders.ExecuteDeleteAsync();
        await context.Books.ExecuteDeleteAsync();
        await context.Clients.ExecuteDeleteAsync();
        await context.Tags.ExecuteDeleteAsync();
    }
}