using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;

namespace DatabaseClient.Repositories.Abstraction;

public interface IBooksRepository : IRepository<Book>
{
    Task<ICollection<Book>> GetBooksByTitleAsync(string title);

    Task<ICollection<Book>> GetBooksByAuthorAsync(string author);

    Task<ICollection<Book>> GetBooksByTagsAsync(IEnumerable<string> tags);

    Task<ICollection<Book>> GetBooksWithCountLessThanAsync(int count);

    Task<Book> AddBookAsync(string title, string author, DateTime releaseDate, int price, int count = 0);

    Task AddTagToBookAsync(Book book, Tag tag);

    Task RemoveTagFromBookAsync(Book book, Tag tag);
}