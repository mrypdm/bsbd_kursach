using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using DatabaseClient.Reports;
using Microsoft.EntityFrameworkCore;

namespace DatabaseClient.Providers;

public class ReportsProvider
{
    public async Task<int> CountOfSales(Book book)
    {
        var context = DatabaseContext.Instance;
        return await context.OrdersToBooks
            .Where(m => m.BookId == book.Id)
            .SumAsync(m => m.Count)
            ;
    }

    public async Task<int> RevenueOfClient(Client client)
    {
        var context = DatabaseContext.Instance;
        return await context.Orders
            .Where(m => m.ClientId == client.Id)
            .SelectMany(m => m.OrdersToBooks)
            .SumAsync(m => m.Count * m.Book.Price)
            ;
    }

    public async Task<double> AverageScoreOfBook(Book book)
    {
        var context = DatabaseContext.Instance;
        return await context.Reviews
            .Where(m => m.BookId == book.Id)
            .AverageAsync(m => m.Score)
            ;
    }

    public async Task<ICollection<ScoredBook>> MostScoredBooks(int topCount = 10)
    {
        var context = DatabaseContext.Instance;
        return await context.Books
            .Select(m => new
            {
                Book = m,
                Score = m.Reviews.Average(r => r.Score)
            })
            .OrderByDescending(m => m.Score)
            .Take(topCount)
            .Select(m => new ScoredBook(m.Book, m.Score))
            .ToListAsync()
            ;
    }

    public async Task<ICollection<SoldBook>> MostPopularBooks(int topCount = 10)
    {
        var context = DatabaseContext.Instance;
        return await context.Books
            .Select(m => new
            {
                Book = m,
                Sum = m.OrdersToBooks.Sum(o => o.Count)
            })
            .OrderByDescending(m => m.Sum)
            .Take(topCount)
            .Select(m => new SoldBook(m.Book, m.Sum))
            .ToListAsync()
            ;
    }

    public async Task<ICollection<RevenueBook>> MostMakeMoneyBooks(int topCount = 10)
    {
        var context = DatabaseContext.Instance;
        return await context.Books
            .Select(m => new
            {
                Book = m,
                Sum = m.OrdersToBooks.Sum(o => o.Count) * m.Price
            })
            .OrderByDescending(m => m.Sum)
            .Take(topCount)
            .Select(m => new RevenueBook(m.Book, m.Sum))
            .ToListAsync()
            ;
    }

    public async Task<ICollection<RevenueClient>> MostMakeMoneyClients(int topCount = 10)
    {
        var context = DatabaseContext.Instance;
        return await context.Clients
            .Select(m => new
            {
                Client = m,
                Sum = m.Orders.SelectMany(order => order.OrdersToBooks.Select(orb => orb.Count * orb.Book.Price)).Sum()
            })
            .OrderByDescending(m => m.Sum)
            .Take(topCount)
            .Select(m => new RevenueClient(m.Client, m.Sum))
            .ToListAsync()
            ;
    }
}