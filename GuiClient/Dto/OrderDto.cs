using System;

namespace GuiClient.Dto;

public class OrderDto
{
    public int Id { get; set; } = -1;

    public int ClientId { get; set; }

    public string Client { get; set; }

    public DateTime CreatedAt { get; set; }
}