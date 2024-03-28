using System;

namespace GuiClient.Dto;

[Serializable]
public sealed class BookDto : NotifyPropertyChanged
{
    private int? _revenue;

    private int? _sales;

    private double? _score;

    public int Id { get; set; } = -1;

    public string Title { get; set; }

    public string Author { get; set; }

    public DateTime ReleaseDate { get; set; } = DateTime.Now;

    public int Count { get; set; }

    public int Price { get; set; }

    public string Tags { get; set; } = string.Empty;

    public int? Sales
    {
        get => _sales;
        set => SetField(ref _sales, value);
    }

    public int? Revenue
    {
        get => _revenue;
        set => SetField(ref _revenue, value);
    }

    public double? Score
    {
        get => _score;
        set => SetField(ref _score, value);
    }

    public override string ToString()
    {
        return $"{Author} - {Title}";
    }
}