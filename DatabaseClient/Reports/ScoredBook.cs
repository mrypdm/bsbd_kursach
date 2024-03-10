using DatabaseClient.Models;

namespace DatabaseClient.Reports;

public class ScoredBook(Book book, double score)
{
    public Book Book { get; } = book;

    public double Score { get; } = score;
}