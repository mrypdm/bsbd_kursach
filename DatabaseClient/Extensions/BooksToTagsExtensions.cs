using System;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;

namespace DatabaseClient.Extensions;

public static class BooksToTagsExtensions
{
    public static async Task AddTagToBook(this DatabaseContext context, Book book, Tag tag)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tag);

        context.Attach(book);
        context.Attach(tag);

        book.Tags.Add(tag);
        context.Update(book);
        await context.SaveChangesAsync();
    }

    public static async Task RemoveTagFromBook(this DatabaseContext context, Book book, Tag tag)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tag);

        context.Attach(book);
        context.Attach(tag);

        book.Tags.Remove(tag);
        context.Update(book);
        await context.SaveChangesAsync();
    }
}