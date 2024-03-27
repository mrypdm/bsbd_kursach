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

        context.TryAttach(book);
        context.TryAttach(tag);

        book.Tags.Add(tag);
        context.Update(book);
        await context.SaveChangesAsync();
    }

    public static async Task RemoveTagFromBook(this DatabaseContext context, Book book, Tag tag)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(book);
        ArgumentNullException.ThrowIfNull(tag);

        context.TryAttach(book);
        context.TryAttach(tag);

        book.Tags.RemoveWhere(m => m.Id == tag.Id);
        tag.Books.RemoveWhere(m => m.Id == book.Id);
        context.Update(book);
        await context.SaveChangesAsync();
    }
}