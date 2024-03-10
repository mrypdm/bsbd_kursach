using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Reports;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Repositories;

public class ReportsRepository
{
    public async Task<int> CountOfSales(Book book)
    {
        var context = DatabaseContext.Instance;
        return await context.OrdersToBooks.Where(m => m.BookId == book.Id).SumAsync(m => m.Count);
    }

    public async Task<int> RevenueOfClient(Client client)
    {
        var context = DatabaseContext.Instance;
        return await context.Orders
            .Where(m => m.ClientId == client.Id)
            .SelectMany(m => m.OrdersToBooks)
            .SumAsync(m => m.Count * m.Book.Price);
    }

    public async Task<double> AverageScoreOfBook(Book book)
    {
        var context = DatabaseContext.Instance;
        return await context.Reviews.Where(m => m.BookId == book.Id).AverageAsync(m => m.Score);
    }

    public async Task<ICollection<ScoredBook>> MostScoredBooks(int topCount = 10)
    {
        var context = DatabaseContext.Instance;
        return await context.Reviews
            .GroupBy(m => m.Book)
            .Select(m => new ScoredBook(m.Key, m.Average(b => b.Score)))
            .OrderByDescending(m => m.Score)
            .Take(topCount)
            .ToListAsync();
    }

    public async Task<ICollection<SoldBook>> MostPopularBooks(int topCount = 10)
    {
        var context = DatabaseContext.Instance;
        return await context.OrdersToBooks
            .GroupBy(m => m.Book)
            .Select(m => new SoldBook(m.Key, m.Sum(b => b.Count)))
            .OrderByDescending(m => m.Count)
            .Take(topCount)
            .ToListAsync();
    }

    public async Task<ICollection<RevenueBook>> MostMakeMoneyBooks(int topCount = 10)
    {
        var context = DatabaseContext.Instance;
        return await context.OrdersToBooks
            .GroupBy(m => m.Book)
            .Select(m => new RevenueBook(m.Key, m.Sum(b => b.Count) * m.Key.Price))
            .OrderByDescending(m => m.TotalSum)
            .Take(topCount)
            .ToListAsync();
    }

    public async Task<ICollection<RevenueClient>> MostMakeMoneyClients(int topCount = 10)
    {
        var context = DatabaseContext.Instance;
        return await context.OrdersToBooks
            .GroupBy(m => m.Order)
            .Select(m => new { m.Key.Client, TotalSum = m.Sum(b => b.Count * b.Book.Price) })
            .GroupBy(m => m.Client)
            .Select(m => new RevenueClient(m.Key, m.Sum(b => b.TotalSum)))
            .OrderByDescending(m => m.TotalSum)
            .Take(topCount)
            .ToListAsync();
    }
}