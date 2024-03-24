using System;
using DatabaseClient.Models;

namespace GuiClient.Dtos;

[Serializable]
public class TagDto : IEntity
{
    public string Name { get; set; }

    public int Id { get; set; } = -1;
}