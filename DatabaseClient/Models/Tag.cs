using System;

namespace DatabaseClient.Models;

[Serializable]
public sealed class Tag
{
    public int Id { get; set; }

    public string Name { get; set; }
}