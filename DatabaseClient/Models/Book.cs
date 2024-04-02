using System;

namespace DatabaseClient.Models;

[Serializable]
public class Book
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTime ReleaseDate { get; set; }

    public int Count { get; set; }

    public int Price { get; set; }

    public bool IsDeleted { get; set; }

    public override string ToString()
    {
        return $"{(IsDeleted ? "(DELETED) " : "")}{Author} - {Title}";
    }
}