﻿using System;
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
        var context = DatabaseContext.Instance;
        return await context.Books.Where(m => m.Title == title).ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksByAuthorAsync(string author)
    {
        var context = DatabaseContext.Instance;
        return await context.Books.Where(m => m.Author == author).ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksByTagsAsync(IEnumerable<string> tags)
    {
        var context = DatabaseContext.Instance;

        var command = tags.Aggregate(context.Books as IQueryable<Book>,
            (current, tag) => current.Where(m => m.Tags.Select(t => t.Title).Contains(tag)));

        return await command.ToListAsync().ConfigureAwait(false);
    }

    public async Task<ICollection<Book>> GetBooksWithCountLessThanAsync(int count)
    {
        var context = DatabaseContext.Instance;
        return await context.Books.Where(m => m.Count < count).ToListAsync().ConfigureAwait(false);
    }

    public async Task<Book> AddBookAsync(string title, string author, DateTime releaseDate, int price, int count = 0)
    {
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

    public async Task UpdateBookAsync(Book book)
    {
        var context = DatabaseContext.Instance;
        context.Update(book);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteBookAsync(Book book)
    {
        var context = DatabaseContext.Instance;
        context.Books.Remove(book);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task AttachTagAsync(Book book, Tag tag)
    {
        book.Tags.Add(tag);
        await UpdateBookAsync(book);
    }

    public async Task DetachTagAsync(Book book, Tag tag)
    {
        if (book.Tags.Remove(tag))
        {
            await UpdateBookAsync(book);
        }
    }
}