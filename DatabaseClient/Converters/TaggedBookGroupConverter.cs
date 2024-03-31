using System.Linq;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;

namespace DatabaseClient.Converters;

public class TaggedBookGroupConverter : IGroupConverter<DbTaggedBook, Book>
{
    public Book Convert(IGrouping<int, DbTaggedBook> group)
    {
        var dbBook = group.First();

        var tags = group
            .Select(m => m.TagId == null ? null : new Tag { Id = m.TagId.Value, Name = m.TagName, Books = null })
            .Where(m => m is not null)
            .ToArray();

        var book = new Book
        {
            Id = dbBook.Id,
            Title = dbBook.Title,
            Author = dbBook.Author,
            ReleaseDate = dbBook.ReleaseDate,
            Count = dbBook.Count,
            Price = dbBook.Price,
            IsDeleted = dbBook.IsDeleted,
            OrdersToBooks = null,
            Reviews = null,
            Tags = tags.Length > 0 ? tags : null
        };

        return book;
    }
}