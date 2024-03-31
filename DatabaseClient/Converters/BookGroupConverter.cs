using System.Linq;
using DatabaseClient.Models;
using DatabaseClient.Models.Internal;

namespace DatabaseClient.Converters;

public class BookGroupConverter : IGroupConverter<DbBook, Book>
{
    public Book Convert(IGrouping<int, DbBook> group)
    {
        var dbBook = group.First();

        var book = new Book
        {
            Id = dbBook.Id,
            Title = dbBook.Title,
            Author = dbBook.Author,
            ReleaseDate = dbBook.ReleaseDate,
            Count = dbBook.Count,
            Price = dbBook.Price,
            IsDeleted = dbBook.IsDeleted
        };

        return book;
    }
}