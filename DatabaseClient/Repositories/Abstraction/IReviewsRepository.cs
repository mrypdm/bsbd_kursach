using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IReviewsRepository : IRepository<Review>
{
    Task<ICollection<Review>> GetReviewForClientAsync(Client client);

    Task<ICollection<Review>> GetReviewForBooksAsync(Book book);

    Task<Review> AddReviewAsync(Client client, Book book, int score, string text = null);
}