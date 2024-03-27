using System;

namespace GuiClient.Dto;

[Serializable]
public class BookDto
{
    public int Id { get; set; } = -1;

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTime ReleaseDate { get; set; } = DateTime.Now;

    public int Count { get; set; }

    public int Price { get; set; }

    public string Tags { get; set; }

    public override string ToString()
    {
        return $"{Author} - {Title}";
    }
}