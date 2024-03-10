using DatabaseClient.Models;

namespace DatabaseClient.Reports;

public class RevenueBook(Book book, int totalSum)
{
    public Book Book { get; } = book;

    public int TotalSum { get; } = totalSum;
}