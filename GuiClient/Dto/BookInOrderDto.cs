namespace GuiClient.Dto;

public class BookInOrderDto
{
    public int OrderId { get; set; }

    public int BookId { get; set; }

    public string Book { get; set; }

    public int Count { get; set; }
}