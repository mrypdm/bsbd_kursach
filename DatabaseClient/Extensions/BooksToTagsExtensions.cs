using System;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Extensions;

public static class BooksToTagsExtensions
{
    public static async Task AddTagToBook(this DatabaseContext context, Book book, Tag tag)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tag);

        await context.Database.ExecuteSqlAsync(
            $"""
             insert into BooksToTags (TagId, BookId)
             values  ({tag.Id}, {book.Id})
             """);
    }

    public static async Task RemoveTagFromBook(this DatabaseContext context, Book book, Tag tag)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tag);

        await context.Database.ExecuteSqlAsync(
            $"""
             delete from BooksToTags
             where BookId = {book.Id} and TagId = {tag.Id}
             """);
    }
}