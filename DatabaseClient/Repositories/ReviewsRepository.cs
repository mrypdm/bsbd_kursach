using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ReviewsRepository
{
    public async Task<ICollection<Review>> GetReviewForClientAsync(Client client)
    {
        var context = DatabaseContext.Instance;
        return await context.Reviews
            .Where(m => m.ClientId == client.Id)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<ICollection<Review>> GetReviewForBooksAsync(Book book)
    {
        var context = DatabaseContext.Instance;
        return await context.Reviews
            .Where(m => m.BookId == book.Id)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<Review> AddReviewAsync(Client client, Book book, int score, string text = null)
    {
        var review = new Review
        {
            ClientId = client.Id,
            BookId = book.Id,
            Score = score,
            Text = text
        };
        
        var context = DatabaseContext.Instance;
        var entity = await context.Reviews.AddAsync(review).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Entity;
    }

    public async Task UpdateReviewAsync(Review review)
    {
        var context = DatabaseContext.Instance;
        context.Update(review);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteReviewAsync(Review review)
    {
        var context = DatabaseContext.Instance;
        context.Reviews.Remove(review);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}