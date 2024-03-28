using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class BooksRepository(DatabaseContextFactory factory) : BaseRepository<Book>(factory), IBooksRepository
{
    // bsbd_mark_book_as_deleted prevents deletion and marks books as deleted and removes book from tags

    public override async Task<Book> GetByIdAsync(int id)
    {
        await using var context = Factory.Create();
        return await context.Books
            .Where(m => m.Id == id && !m.IsDeleted)
            .Include(m => m.Tags)
            .SingleOrDefaultAsync();
    }

    public override async Task<ICollection<Book>> GetAllAsync()
    {
        await using var context = Factory.Create();
        return await context.Books
            .Where(m => !m.IsDeleted)
            .Include(m => m.Tags)
            .ToListAsync();
    }

    public async Task<ICollection<Book>> GetBooksByTitleAsync(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        await using var context = Factory.Create();
        return await context.Books
            .Where(m => m.Title == title && !m.IsDeleted)
            .Include(m => m.Tags)
            .ToListAsync();
    }

    public async Task<ICollection<Book>> GetBooksByAuthorAsync(string author)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        await using var context = Factory.Create();
        return await context.Books
            .Where(m => m.Author == author && !m.IsDeleted)
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

        var command = safeTags.Aggregate(context.Books.Where(m => !m.IsDeleted),
            (current, tag) => current.Where(m => m.Tags.Select(t => t.Name).Contains(tag)));

        return await command.Include(m => m.Tags).ToListAsync();
    }

    public async Task<ICollection<Book>> GetBooksWithCountLessThanAsync(int count)
    {
        await using var context = Factory.Create();
        return await context.Books
            .Where(m => m.Count < count && !m.IsDeleted)
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

    public override async Task UpdateAsync(Book entity)
    {
        await using var context = Factory.Create();
        await context.Books
            .Where(m => m.Id == entity.Id)
            .ExecuteUpdateAsync(o => o
                .SetProperty(m => m.Title, entity.Title)
                .SetProperty(m => m.Author, entity.Author)
                .SetProperty(m => m.ReleaseDate, entity.ReleaseDate)
                .SetProperty(m => m.Count, entity.Count)
                .SetProperty(m => m.Price, entity.Price));
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

    public async Task<int> CountOfSales(Book book)
    {
        await using var context = Factory.Create();
        return await context.OrdersToBooks
            .Where(m => m.BookId == book.Id && !m.Book.IsDeleted)
            .SumAsync(m => m.Count);
    }

    public async Task<int> RevenueOfBook(Book book)
    {
        await using var context = Factory.Create();
        return await context.OrdersToBooks
            .Where(m => m.BookId == book.Id && !m.Book.IsDeleted)
            .SumAsync(m => m.Count * m.Book.Price);
    }

    public async Task<double> ScoreOfBook(Book book)
    {
        await using var context = Factory.Create();
        return await context.Reviews
            .Where(m => m.BookId == book.Id && !m.Book.IsDeleted)
            .AverageAsync(m => m.Score);
    }

    public async Task<ICollection<Book>> MostScoredBooks(int topCount = 10)
    {
        await using var context = Factory.Create();
        return await context.Books
            .Where(m => !m.IsDeleted)
            .Include(m => m.Tags)
            .Select(m => new
            {
                Book = m,
                Score = m.Reviews.Average(r => r.Score)
            })
            .OrderByDescending(m => m.Score)
            .Take(topCount)
            .Select(m => m.Book)
            .ToListAsync();
    }

    public async Task<ICollection<Book>> MostSoldBooks(int topCount = 10)
    {
        await using var context = Factory.Create();
        return await context.Books
            .Where(m => !m.IsDeleted)
            .Include(m => m.Tags)
            .Select(m => new
            {
                Book = m,
                Sum = m.OrdersToBooks.Sum(o => o.Count)
            })
            .OrderByDescending(m => m.Sum)
            .Take(topCount)
            .Select(m => m.Book)
            .ToListAsync();
    }

    public async Task<ICollection<Book>> MostRevenueBooks(int topCount = 10)
    {
        await using var context = Factory.Create();
        return await context.Books
            .Where(m => !m.IsDeleted)
            .Include(m => m.Tags)
            .Select(m => new
            {
                Book = m,
                Sum = m.OrdersToBooks.Sum(o => o.Count) * m.Price
            })
            .OrderByDescending(m => m.Sum)
            .Take(topCount)
            .Select(m => m.Book)
            .ToListAsync();
    }
}