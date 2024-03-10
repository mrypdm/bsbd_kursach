using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;

namespace DatabaseClient.Extensions;

public static class BooksToTagsExtensions
{
    public static async Task AddTagToBook(this DatabaseContext context, Book book, Tag tag)
    {
        book.Tags.Add(tag);
        tag.Books.Add(book);
        context.Update(book);
        context.Update(tag);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    public static async Task RemoveTagFromBook(this DatabaseContext context, Book book, Tag tag)
    {
        book.Tags.Remove(tag);
        tag.Books.Remove(book);
        context.Update(book);
        context.Update(tag);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }
}