using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class BooksRepository : BaseRepository<Book>
{
    public async Task<ICollection<Book>> GetBooksByNameAsync(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        var context = DatabaseContext.Instance;
        return await context.Books
            .Where(m => m.Title == title)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksByAuthorAsync(string author)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        var context = DatabaseContext.Instance;
        return await context.Books
            .Where(m => m.Author == author)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksByTagsAsync(IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        var safeTags = tags.Where(m => m is not null).ToList();
        if (safeTags.Count == 0)
        {
            throw new ArgumentException("Tags are empty");
        }

        var context = DatabaseContext.Instance;

        var command = safeTags.Aggregate(context.Books as IQueryable<Book>,
            (current, tag) => current.Where(m => m.Tags.Select(t => t.Title).Contains(tag)));

        return await command.ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksWithCountLessThanAsync(int count)
    {
        var context = DatabaseContext.Instance;
        return await context.Books
            .Where(m => m.Count < count)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    public async Task<Book> AddBookAsync(string title, string author, DateTime releaseDate, int price, int count = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        var book = new Book
        {
            Title = title,
            Author = author,
            ReleaseDate = releaseDate,
            Price = price,
            Count = count
        };

        var context = DatabaseContext.Instance;
        var entity = await context.Books.AddAsync(book).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Entity;
    }

    public async Task AddTagToBookAsync(Book book, Tag tag)
    {
        var context = DatabaseContext.Instance;
        await context.AddTagToBook(book, tag).ConfigureAwait(false);
    }

    public async Task RemoveTagFromBookAsync(Book book, Tag tag)
    {
        var context = DatabaseContext.Instance;
        await context.RemoveTagFromBook(book, tag).ConfigureAwait(false);
    }
}