using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class BooksRepository(DatabaseContextFactory factory) : BaseRepository<Book>(factory)
{
    public override async Task<ICollection<Book>> GetAllAsync()
    {
        await using var context = Factory.Create();
        return await context.Books
            .Include(m => m.Tags)
            .ToListAsync();
    }

    public async Task<ICollection<Book>> GetBooksByTitleAsync(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        await using var context = Factory.Create();
        return await context.Books
            .Where(m => m.Title == title)
            .Include(m => m.Tags)
            .ToListAsync();
    }

    public async Task<ICollection<Book>> GetBooksByAuthorAsync(string author)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        await using var context = Factory.Create();
        return await context.Books
            .Where(m => m.Author == author)
            .Include(m => m.Tags)
            .ToListAsync();
    }

    public async Task<ICollection<Book>> GetBooksByTagsAsync(IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        var safeTags = tags.Where(m => m is not null).ToList();
        if (safeTags.Count == 0)
        {
            throw new ArgumentException("Tags are empty");
        }

        await using var context = Factory.Create();

        var command = safeTags.Aggregate(context.Books as IQueryable<Book>,
            (current, tag) => current.Where(m => m.Tags.Select(t => t.Title).Contains(tag)));

        return await command.Include(m => m.Tags).ToListAsync();
    }

    public async Task<ICollection<Book>> GetBooksWithCountLessThanAsync(int count)
    {
        await using var context = Factory.Create();
        return await context.Books
            .Where(m => m.Count < count)
            .Include(m => m.Tags)
            .ToListAsync();
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

        await using var context = Factory.Create();
        var entity = await context.Books.AddAsync(book);
        await context.SaveChangesAsync();
        return entity.Entity;
    }

    public async Task AddTagToBookAsync(Book book, Tag tag)
    {
        await using var context = Factory.Create();
        await context.AddTagToBook(book, tag);
    }

    public async Task RemoveTagFromBookAsync(Book book, Tag tag)
    {
        await using var context = Factory.Create();
        await context.RemoveTagFromBook(book, tag);
    }
}