using System.Diagnostics.CodeAnalysis;
using DatabaseClient.Models;
using DatabaseClient.Repositories;

namespace ConsoleClient;

[SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task")]
[SuppressMessage("Security", "CA5394:Do not use insecure randomness")]
public static class Startup
{
    public static async Task InitDataBase()
    {
        var clientsRepository = new ClientsRepository();
        var tagsRepository = new TagsRepository();
        var booksRepository = new BooksRepository();
        var reviewsRepository = new ReviewsRepository();
        var ordersRepository = new OrdersRepository();

        var cl1 = await clientsRepository.AddClientAsync("Maxim", "Shapovalov", "0987654321");
        var cl2 = await clientsRepository.AddClientAsync("Andrew", "Raficoff", "8005553535");
        var cl3 = await clientsRepository.AddClientAsync("Vitya", "Aka", "7777777777");
        var cl4 = await clientsRepository.AddClientAsync("Vasiliy", "Skorobogadko", "0000000000");
        var cl5 = await clientsRepository.AddClientAsync("Admin", "Adminovich", "1234567890");

        var tag1 = await tagsRepository.AddTagAsync("action");
        var tag2 = await tagsRepository.AddTagAsync("fanfiction");
        var tag3 = await tagsRepository.AddTagAsync("history");
        var tag4 = await tagsRepository.AddTagAsync("romance");
        var tag5 = await tagsRepository.AddTagAsync("science");

        var b1 = await booksRepository.AddBookAsync("Very interesting book", "Famous Author", DateTime.Now, 1000, 123);
        var b2 = await booksRepository.AddBookAsync("Very interesting book 2", "Famous Author", DateTime.Now.AddDays(3),
            1300,
            91);
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
}