using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ReviewsRepository(DatabaseContextFactory factory) : IReviewsRepository
{
    public async Task<ICollection<Review>> GetReviewForClientAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        await using var context = factory.Create();
        return await context.Reviews
            .Where(m => m.ClientId == client.Id)
            .Include(m => m.Book)
            .Include(m => m.Client)
            .ToListAsync();
    }

    public async Task<ICollection<Review>> GetReviewForBooksAsync(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = factory.Create();
        return await context.Reviews
            .Where(m => m.BookId == book.Id)
            .Include(m => m.Book)
            .Include(m => m.Client)
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

        await using var context = factory.Create();
        var entity = await context.Reviews.AddAsync(review);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task<Review> GetByIdAsync(int bookId, int clientId)
    {
        await using var context = factory.Create();
        return await context.Reviews
            .Where(m => m.BookId == bookId && m.ClientId == clientId)
            .SingleOrDefaultAsync();
    }

    public Task<Review> GetByIdAsync(int id)
    {
        throw new NotSupportedException();
    }

    public async Task<ICollection<Review>> GetAllAsync()
    {
        await using var context = factory.Create();
        return await context.Reviews
            .Include(m => m.Book)
            .Include(m => m.Client)
            .ToArrayAsync();
    }

    public async Task UpdateAsync(Review entity)
    {
        await using var context = factory.Create();
        await context.Reviews
            .Where(m => m.BookId == entity.BookId && m.ClientId == entity.ClientId)
            .ExecuteUpdateAsync(o => o
                .SetProperty(m => m.Text, entity.Text)
                .SetProperty(m => m.Score, entity.Score));
    }

    public async Task RemoveAsync(Review entity)
    {
        if (entity == null)
        {
            return;
        }

        await using var context = factory.Create();
        await context.Reviews
            .Where(m => m.BookId == entity.BookId && m.ClientId == entity.ClientId)
            .ExecuteDeleteAsync();
    }
}