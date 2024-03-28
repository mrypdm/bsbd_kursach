using DatabaseClient.Models;

namespace GuiClient.Dto;

public class ClientDto
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public Gender Gender { get; set; }

    public int OrdersCount { get; set; }

    public int Revenue { get; set; }

    public override string ToString()
    {
        return $"{FirstName} {LastName} / {Phone}";
    }
}