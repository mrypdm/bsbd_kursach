using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ReviewsRepository(DatabaseContextFactory factory) : BaseRepository<Review>(factory)
{
    public async Task<ICollection<Review>> GetReviewForClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = Factory.Create();
        return await context.Reviews
            .Where(m => m.ClientId == client.Id)
            .ToListAsync();
    }

    public async Task<ICollection<Review>> GetReviewForBooksAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = Factory.Create();
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

        await using var context = Factory.Create();
        var entity = await context.Reviews.AddAsync(review);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public override async Task UpdateAsync(Review entity)
    {
        await using var context = Factory.Create();
        await context.Reviews
            .Where(m => m.Id == entity.Id)
            .ExecuteUpdateAsync(o => o
                .SetProperty(m => m.Text, entity.Text)
                .SetProperty(m => m.Score, entity.Score));
    }
}