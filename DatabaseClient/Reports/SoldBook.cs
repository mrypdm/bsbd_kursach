using DatabaseClient.Models;

namespace DatabaseClient.Reports;

public class SoldBook(Book book, int count)
{
    public Book Book { get; } = book;

    public int Count { get; } = count;
}