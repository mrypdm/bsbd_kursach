using System;

namespace DatabaseClient.Models.Internal;

[Serializable]
public sealed class DbTaggedBook : IDbEntity
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTime ReleaseDate { get; set; }

    public int Count { get; set; }

    public int Price { get; set; }

    public bool IsDeleted { get; set; }

    public int? TagId { get; set; }

    public string TagName { get; set; }
}