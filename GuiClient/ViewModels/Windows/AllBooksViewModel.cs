using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class AllBooksViewModel : AllEntitiesViewModel<Book, BookDto>
{
    private readonly IBooksRepository _booksRepository;
    private readonly ITagsRepository _tagsRepository;
    private readonly IClientsRepository _clientsRepository;

    public AllBooksViewModel(ISecurityContext securityContext, IBooksRepository booksRepository,
        ITagsRepository tagsRepository, IClientsRepository clientsRepository, IMapper mapper)
        : base(securityContext, booksRepository, mapper)
    {
        _booksRepository = booksRepository;
        _tagsRepository = tagsRepository;
        _clientsRepository = clientsRepository;

        ShowReviews = new AsyncFuncCommand<BookDto>(ShowReviewsAsync, item => item?.Id != -1);
        ShowOrders = new AsyncFuncCommand<BookDto>(ShowOrdersAsync, item => item?.Id != -1);

        ShowRevenue = new AsyncFuncCommand<BookDto>(
            async item => { item.Revenue = await _booksRepository.RevenueOfBook(new Book { Id = item.Id }); },
            item => item is { Id: not -1, Revenue: null });
        ShowScore = new AsyncFuncCommand<BookDto>(
            async item => { item.Score = await _booksRepository.ScoreOfBook(new Book { Id = item.Id }); },
            item => item is { Id: not -1, Score: null });
        ShowSales = new AsyncFuncCommand<BookDto>(
            async item => { item.Sales = await _booksRepository.CountOfSales(new Book { Id = item.Id }); },
            item => item is { Id: not -1, Sales: null });
        ShowTags = new AsyncFuncCommand<BookDto>(ShowTagsAsync, item => item is { Id: not -1 });
    }

    public ICommand ShowReviews { get; }

    public ICommand ShowOrders { get; }

    public ICommand ShowRevenue { get; }

    public ICommand ShowTags { get; }

    public ICommand ShowScore { get; }

    public ICommand ShowSales { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);

        if (IsWorker)
        {
            AddButton(window, "Update", nameof(Update));
            AddButton(window, "Show reviews", nameof(ShowReviews));
            AddButton(window, "Show orders", nameof(ShowOrders));
        }

        AddText(window, nameof(BookDto.Id), true);
        AddText(window, nameof(BookDto.Title));
        AddText(window, nameof(BookDto.Author));
        AddText(window, nameof(BookDto.ReleaseDate));
        AddText(window, nameof(BookDto.Count));
        AddText(window, nameof(BookDto.Price));
        AddButton(window, nameof(BookDto.Sales), nameof(ShowSales), true);
        AddButton(window, nameof(BookDto.Revenue), nameof(ShowRevenue), true);
        AddButton(window, nameof(BookDto.Score), nameof(ShowScore), true);
        AddButton(window, nameof(BookDto.Tags), nameof(ShowTags), true);
    }

    protected override async Task UpdateAsync([NotNull] BookDto item)
    {
        if (item.Id == -1)
        {
            var book = await _booksRepository.AddBookAsync(item.Title, item.Author, item.ReleaseDate, item.Price,
                item.Count);
            MessageBox.Show($"Book created with ID={book.Id}");
        }
        else
        {
            var book = await _booksRepository.GetByIdAsync(item.Id);
            book.Title = item.Title;
            book.Author = item.Author;
            book.ReleaseDate = item.ReleaseDate;
            book.Count = item.Count;
            book.Price = item.Price;

            await _booksRepository.UpdateAsync(book);

            if (item.Tags != null)
            {
                var newTags = item.Tags
                    .Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                var currentTags = book.Tags.Select(m => m.Name).ToArray();
                var toDelete = currentTags.Except(newTags);
                var toAdd = newTags.Except(currentTags);

                await _tagsRepository.RemoveBookFromTags(book, toDelete);
                await _tagsRepository.AddBookToTags(book, toAdd);
            }
        }

        await RefreshAsync();
    }

    private async Task ShowReviewsAsync(BookDto book)
    {
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<Review, ReviewDto>>();

        await allReviews.ShowBy(
            async (r, m) =>
            {
                var repo = r.Cast<Review, IReviewsRepository>();
                return m.Map<ReviewDto[]>(await repo.GetReviewForBooksAsync(new Book { Id = book.Id }));
            },
            async () =>
            {
                if (!AskerWindow.TryAskInt("Enter client ID", out var clientId))
                {
                    return null;
                }

                var client = await _clientsRepository.GetByIdAsync(clientId)
                    ?? throw new KeyNotFoundException($"Cannot find client with Id={clientId}");

                return new ReviewDto
                {
                    BookId = book.Id,
                    Book = book.ToString(),
                    ClientId = client.Id,
                    Client = client.ToString()
                };
            });
    }

    private async Task ShowOrdersAsync(BookDto book)
    {
        var allOrders = App.ServiceProvider.GetRequiredService<IEntityViewModel<Order, OrderDto>>();

        await allOrders.ShowBy(
            async (r, m) =>
            {
                var repo = r.Cast<Order, IOrdersRepository>();
                return m.Map<OrderDto[]>(await repo.GetOrdersForBookAsync(new Book { Id = book.Id }));
            },
            async () =>
            {
                if (!AskerWindow.TryAskInt("Enter client ID", out var clientId))
                {
                    return null;
                }

                var client = await _clientsRepository.GetByIdAsync(clientId)
                    ?? throw new KeyNotFoundException($"Cannot find book with Id={clientId}");

                return new OrderDto
                {
                    ClientId = client.Id,
                    Client = client.ToString(),
                    CreatedAt = DateTime.Now,
                    Books =
                    [
                        new BookInOrderDto
                        {
                            OrderId = -1,
                            BookId = book.Id,
                            Book = book.ToString(),
                            Count = 1,
                            Price = book.Price
                        }
                    ]
                };
            });
    }

    private async Task ShowTagsAsync(BookDto book)
    {
        if (book.Tags == null)
        {
            var tags = await _tagsRepository.GetTagsOfBook(new Book { Id = book.Id });
            book.Tags = string.Join(", ", tags.Select(m => m.Name));
        }
        else
        {
            var newTags = AskerWindow.AskString("Enter tags separated by comma", book.Tags);
            book.Tags = newTags ?? book.Tags;
        }
    }
}