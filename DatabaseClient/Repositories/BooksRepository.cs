using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;
using DatabaseClient.Repositories.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class BooksRepository(DatabaseContextFactory factory) : IBooksRepository
{
    // bsbd_mark_book_as_deleted prevents deletion and marks books as deleted and removes book from tags

    public async Task<Book> GetByIdAsync(int id)
    {
        await using var context = factory.Create();

        // return await context.Books
        //     .Where(m => m.Id == id && !m.IsDeleted)
        //     .Include(m => m.Tags)
        //     .SingleOrDefaultAsync();

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 select b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                 from Books b
                 where b.IsDeleted = 0 and b.Id = {id}
                 order by b.Id
                 """)
            .SingleOrDefaultAsync<DbBook, Book>();
    }

    public async Task<ICollection<Book>> GetAllAsync()
    {
        await using var context = factory.Create();

        // return await context.Books
        //     .Where(m => !m.IsDeleted)
        //     .Include(m => m.Tags)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 select b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                 from Books b
                 where b.IsDeleted = 0
                 order by b.Id
                 """)
            .AsListAsync<DbBook, Book>();
    }

    public async Task<ICollection<Book>> GetBooksByTitleAsync(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        await using var context = factory.Create();

        // return await context.Books
        //     .Where(m => m.Title == title && !m.IsDeleted)
        //     .Include(m => m.Tags)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 select b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                 from Books b
                 where b.IsDeleted = 0 and b.Title = {title}
                 order by b.Id
                 """)
            .AsListAsync<DbBook, Book>();
    }

    public async Task<ICollection<Book>> GetBooksByAuthorAsync(string author)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        await using var context = factory.Create();

        // return await context.Books
        //     .Where(m => m.Author == author && !m.IsDeleted)
        //     .Include(m => m.Tags)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 select b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                 from Books b
                 where b.IsDeleted = 0 and b.Author = {author}
                 order by b.Id
                 """)
            .AsListAsync<DbBook, Book>();
    }

    public async Task<ICollection<Book>> GetBooksByTagsAsync(IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);

        var safeTags = tags.Where(m => m is not null).ToList();
        if (safeTags.Count == 0)
        {
            throw new ArgumentException("Tags are empty");
        }

        await using var context = factory.Create();

        // var command = safeTags.Aggregate(context.Books.Where(m => !m.IsDeleted),
        //     (current, tag) => current.Where(m => m.Tags.Select(t => t.Name).Contains(tag)));
        //
        // return await command.Include(m => m.Tags).ToListAsync();

        var condition = string.Join(" and ", safeTags.Select((_, i) =>
            $"@tag{i} in (select t.Name from BooksToTags btt join Tags t on btt.TagId = t.Id where b.Id = btt.BookId)"));
        var args = safeTags.Select((m, i) => new SqlParameter($"tag{i}", m)).ToArray();

        var query = $"""
                     select b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                     from Books b
                     where b.IsDeleted = 0 and {condition}
                     order by b.Id
                     """;

        // ReSharper disable once CoVariantArrayConversion
        return await context.Database
            .SqlQueryRaw<DbBook>(query, args)
            .AsListAsync<DbBook, Book>();
    }

    public async Task<ICollection<Book>> GetBooksWithCountLessThanAsync(int count)
    {
        await using var context = factory.Create();

        // return await context.Books
        //     .Where(m => m.Count < count && !m.IsDeleted)
        //     .Include(m => m.Tags)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 select b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                 from Books b
                 where b.IsDeleted = 0 and b.Count < {count}
                 order by b.Id
                 """)
            .AsListAsync<DbBook, Book>();
    }

    public async Task<Book> AddBookAsync(string title, string author, DateTime releaseDate, int price, int count = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(author);

        await using var context = factory.Create();

        // var book = new Book
        // {
        //     Title = title,
        //     Author = author,
        //     ReleaseDate = releaseDate,
        //     Price = price,
        //     Count = count
        // };
        //
        // var entity = await context.Books.AddAsync(book);
        // await context.SaveChangesAsync();
        // return entity.Entity;

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 insert into Books (Title, Author, ReleaseDate, Count, Price)
                 output inserted.*
                 values ({title}, {author}, {releaseDate}, {price}, {count})
                 """)
            .SingleOrDefaultAsync<DbBook, Book>();
    }

    public async Task UpdateAsync(Book entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await using var context = factory.Create();

        // await context.Books
        //     .Where(m => m.Id == entity.Id)
        //     .ExecuteUpdateAsync(o => o
        //         .SetProperty(m => m.Title, entity.Title)
        //         .SetProperty(m => m.Author, entity.Author)
        //         .SetProperty(m => m.ReleaseDate, entity.ReleaseDate)
        //         .SetProperty(m => m.Count, entity.Count)
        //         .SetProperty(m => m.Price, entity.Price));

        await context.Database.ExecuteSqlAsync(
            $"""
             update Books
             set Title = {entity.Title}, Author = {entity.Author}, ReleaseDate = {entity.ReleaseDate},
                 Count = {entity.Count}, Price = {entity.Price}
             where Id = {entity.Id}
             """);
    }

    public async Task RemoveAsync(Book entity)
    {
        if (entity == null)
        {
            return;
        }

        await using var context = factory.Create();
        await context.Database.ExecuteSqlAsync($"delete from Books where Id == {entity.Id}");
    }

    public async Task AddTagToBookAsync(Book book, Tag tag)
    {
        await using var context = factory.Create();
        await context.AddTagToBook(book, tag);
    }

    public async Task RemoveTagFromBookAsync(Book book, Tag tag)
    {
        await using var context = factory.Create();
        await context.RemoveTagFromBook(book, tag);
    }

    public async Task<int> CountOfSales(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = factory.Create();

        // return await context.OrdersToBooks
        //     .Where(m => m.BookId == book.Id && !m.Book.IsDeleted)
        //     .SumAsync(m => m.Count);

        return await context.Database
            .SqlQuery<int>(
                $"""
                 select coalesce(sum(o.Count), 0) as Value
                 from OrdersToBooks o
                 join Books b on o.BookId = b.Id
                 where o.BookId = {book.Id} and b.IsDeleted = 0
                 """)
            .SingleOrDefaultAsync();
    }

    public async Task<int> RevenueOfBook(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = factory.Create();

        // return await context.OrdersToBooks
        //     .Where(m => m.BookId == book.Id && !m.Book.IsDeleted)
        //     .SumAsync(m => m.Count * m.Book.Price);

        return await context.Database
            .SqlQuery<int>(
                $"""
                 select coalesce(sum(o.Count * b.Price), 0) as Value
                 from OrdersToBooks o
                 join Books b on o.BookId = b.Id
                 where o.BookId = {book.Id} and b.IsDeleted = 0
                 """)
            .SingleOrDefaultAsync();
    }

    public async Task<double> ScoreOfBook(Book book)
    {
        ArgumentNullException.ThrowIfNull(book);

        await using var context = factory.Create();

        // return await context.Reviews
        //     .Where(m => m.BookId == book.Id && !m.Book.IsDeleted)
        //     .AverageAsync(m => m.Score);

        return await context.Database
            .SqlQuery<double>(
                $"""
                 select avg(cast(r.Score as float)) as Value
                 from Reviews r
                 join Books b on r.BookId = b.Id
                 where r.BookId = {book.Id} and b.IsDeleted = 0
                 """)
            .SingleOrDefaultAsync();
    }

    public async Task<ICollection<Book>> MostScoredBooks(int topCount = 10)
    {
        await using var context = factory.Create();

        // return await context.Books
        //     .Where(m => !m.IsDeleted)
        //     .Include(m => m.Tags)
        //     .Select(m => new
        //     {
        //         Book = m,
        //         Score = m.Reviews.Average(r => r.Score)
        //     })
        //     .OrderByDescending(m => m.Score)
        //     .Take(topCount)
        //     .Select(m => m.Book)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 select top({topCount}) b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                 from Books b
                 where b.IsDeleted = 0
                 order by
                 (
                    coalesce((select avg(cast(r.Score as float)) from Reviews r where b.Id = r.BookId), 0)
                 ) desc, b.Id
                 """)
            .AsListAsync<DbBook, Book>();
    }

    public async Task<ICollection<Book>> MostSoldBooks(int topCount = 10)
    {
        await using var context = factory.Create();

        // return await context.Books
        //     .Where(m => !m.IsDeleted)
        //     .Include(m => m.Tags)
        //     .Select(m => new
        //     {
        //         Book = m,
        //         Sum = m.OrdersToBooks.Sum(o => o.Count)
        //     })
        //     .OrderByDescending(m => m.Sum)
        //     .Take(topCount)
        //     .Select(m => m.Book)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 select top({topCount}) b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                 from Books b
                 where b.IsDeleted = 0
                 order by
                 (
                    coalesce((select coalesce(sum(o.Count), 0) from OrdersToBooks o where b.Id = o.BookId), 0)
                 ) desc, b.Id
                 """)
            .AsListAsync<DbBook, Book>();
    }

    public async Task<ICollection<Book>> MostRevenueBooks(int topCount = 10)
    {
        await using var context = factory.Create();

        // return await context.Books
        //     .Where(m => !m.IsDeleted)
        //     .Select(m => new
        //     {
        //         Book = m,
        //         Sum = m.OrdersToBooks.Sum(o => o.Count) * m.Price
        //     })
        //     .OrderByDescending(m => m.Sum)
        //     .Take(topCount)
        //     .Select(m => m.Book)
        //     .ToListAsync();

        return await context.Database
            .SqlQuery<DbBook>(
                $"""
                 select top({topCount}) b.Id, b.Title, b.Author, b.ReleaseDate, b.IsDeleted, b.Count, b.Price
                 from Books b
                 where b.IsDeleted = 0
                 order by
                 (
                    coalesce((select coalesce(sum(o.Count), 0) from OrdersToBooks o where b.Id = o.BookId), 0) * b.Price
                 ) desc, b.Id
                 """)
            .AsListAsync<DbBook, Book>();
    }
}