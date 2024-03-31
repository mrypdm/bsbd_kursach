using System;

namespace DatabaseClient.Models.Internal;

[Serializable]
public class DbOrderToBook : IDbEntity<OrdersToBook>
{
    public int BookId { get; set; }

    public int OrderId { get; set; }

    public int OrderedCount { get; set; }

    public string BookTitle { get; set; }

    public string BookAuthor { get; set; }

    public DateTime BookReleaseDate { get; set; }

    public int BookPrice { get; set; }

    public int TotalCount { get; set; }

    public bool IsBookDeleted { get; set; }

    public OrdersToBook ToEntity()
    {
        return new OrdersToBook
        {
            BookId = BookId,
            OrderId = OrderId,
            Count = OrderedCount,
            Book = new Book
            {
                Id = BookId,
                Title = BookTitle,
                Author = BookAuthor,
                ReleaseDate = BookReleaseDate,
                Count = TotalCount,
                Price = BookPrice,
                IsDeleted = IsBookDeleted
            }
        };
    }
}