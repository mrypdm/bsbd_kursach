using System;
using System.Threading.Tasks;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Extensions;

public static class BooksToTagsExtensions
{
    public static async Task AddTagToBook(this DbContext context, Book book, Tag tag)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tag);

        await context.Database.ExecuteSqlAsync(
            $"""
             insert into BooksToTags (BookId, TagId)
             values ({book.Id}, {tag.Id})
             """);
    }

    public static async Task RemoveTagFromBook(this DbContext context, Book book, Tag tag)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tag);

        await context.Database.ExecuteSqlAsync($"delete from BookToTags where BookId = {book.Id} and TagId = {tag.Id}");
    }
}