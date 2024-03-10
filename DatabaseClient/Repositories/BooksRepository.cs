using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class BooksRepository
{
    public async Task<ICollection<Book>> GetBooksByNameAsync(string title)
    {
        var context = DatabaseContextFactory.Context;
        return await context.Books.Where(m => m.Title == title).ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksByAuthorAsync(string author)
    {
        var context = DatabaseContextFactory.Context;
        return await context.Books.Where(m => m.Author == author).ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksByTagsAsync(IEnumerable<string> tags)
    {
        var context = DatabaseContextFactory.Context;

        var command = tags.Aggregate(context.Books as IQueryable<Book>,
            (current, tag) => current.Where(m => m.Tags.Select(t => t.Title).Contains(tag)));

        return await command.ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksWithCountLessThanAsync(int count)
    {
        var context = DatabaseContextFactory.Context;
        return await context.Books.Where(m => m.Count < count).ToListAsync().ConfigureAwait(false);
    }

    public async Task<Book> AddBookAsync(string title, string author, DateTime releaseDate)
    {
        var book = new Book
        {
            Title = title,
            Author = author,
            ReleaseYear = releaseDate
        };

        var context = DatabaseContextFactory.Context;
        var entity = await context.Books.AddAsync(book).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);
        return entity.Entity;
    }

    public async Task IncreaseBooksCount(Book book, int count)
    {
        var context = DatabaseContextFactory.Context;

        await context.Books
            .Where(m => m.Id == book.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.Count, m => m.Count + count))
            .ConfigureAwait(false);

        await context.Entry(book).ReloadAsync();
    }

    public async Task SetPriceAsync(Book book, int price)
    {
        var context = DatabaseContextFactory.Context;

        await context.Books
            .Where(m => m.Id == book.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.Price, price))
            .ConfigureAwait(false);

        await context.Entry(book).ReloadAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
        var context = DatabaseContextFactory.Context;
        context.Update(book);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteBookAsync(Book book)
    {
        var context = DatabaseContextFactory.Context;
        context.Books.Remove(book);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}