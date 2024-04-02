using DatabaseClient.Models;

namespace GuiClient.ViewModels.Data;

public class ClientDataViewModel : NotifyPropertyChanged
{
    private int? _revenue;

    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Phone { get; set; }

    public Gender Gender { get; set; }

    public int OrdersCount { get; set; }

    public int? Revenue
    {
        get => _revenue;
        set => SetField(ref _revenue, value);
    }

    public override string ToString()
    {
        return Phone != "0000000000" ? $"{FirstName} {LastName} / {Phone}" : "DELETED CLIENT";
    }
}