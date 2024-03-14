using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ReviewsRepository : BaseRepository<Review>
{
    public async Task<ICollection<Review>> GetReviewForClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        var context = DatabaseContext.Instance;
        return await context.Reviews
            .Where(m => m.ClientId == client.Id)
            .ToListAsync();
    }

    public async Task<ICollection<Review>> GetReviewForBooksAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        var context = DatabaseContext.Instance;
        return await context.Reviews
            .Where(m => m.BookId == book.Id)
            .ToListAsync();
    }

    public async Task<Review> AddReviewAsync(Client client, Book book, int score, string text = null)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(book);

        var review = new Review
        {
            ClientId = client.Id,
            BookId = book.Id,
            Score = score,
            Text = text
        };

        var context = DatabaseContext.Instance;
        var entity = await context.Reviews.AddAsync(review);
        await context.SaveChangesAsync();
        return entity.Entity;
    }
}