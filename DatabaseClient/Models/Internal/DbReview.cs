using System;

namespace DatabaseClient.Models.Internal;

[Serializable]
public class DbReview
{
    public int BookId { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTime ReleaseDate { get; set; }

    public int Price { get; set; }

    public int Count { get; set; }

    public bool IsBookDeleted { get; set; }

    public int ClientId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public Gender Gender { get; set; }

    public bool IsClientDeleted { get; set; }

    public int Score { get; set; }

    public string Text { get; set; }
}