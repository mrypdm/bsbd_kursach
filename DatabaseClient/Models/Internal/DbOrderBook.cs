using System;

namespace DatabaseClient.Models.Internal;

[Serializable]
public class DbOrderBook
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
}