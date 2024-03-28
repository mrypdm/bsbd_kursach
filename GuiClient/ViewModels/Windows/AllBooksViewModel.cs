using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Providers;
using DatabaseClient.Repositories;
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
    private readonly TagsRepository _tagsRepository;
    private readonly IClientsRepository _clientsRepository;
    private readonly ReportsProvider _reportsProvider;

    public AllBooksViewModel(ISecurityContext securityContext, IBooksRepository booksRepository,
        TagsRepository tagsRepository, IClientsRepository clientsRepository, ReportsProvider reportsProvider,
        IMapper mapper)
        : base(securityContext, booksRepository, mapper)
    {
        _booksRepository = booksRepository;
        _tagsRepository = tagsRepository;
        _clientsRepository = clientsRepository;
        _reportsProvider = reportsProvider;

        ShowReviews = new AsyncFuncCommand<BookDto>(ShowReviewsAsync, item => item?.Id != -1);
        ShowOrders = new AsyncFuncCommand<BookDto>(ShowOrdersAsync, item => item?.Id != -1);
    }

    public ICommand ShowReviews { get; }

    public ICommand ShowOrders { get; }

    public override async Task RefreshAsync()
    {
        var entities = await Filter(_booksRepository);
        var dtos = Mapper.Map<BookDto[]>(entities);

        for (var i = 0; i < entities.Count; ++i)
        {
            dtos[i].Score = await _reportsProvider.AverageScoreOfBook(entities.ElementAt(i));
            dtos[i].Revenue = await _reportsProvider.RevenueOfBook(entities.ElementAt(i));
            dtos[i].Sold = await _reportsProvider.CountOfSales(entities.ElementAt(i));
        }

        Entities = new ObservableCollection<BookDto>(dtos);
    }

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
        AddText(window, nameof(BookDto.Sold), true);
        AddText(window, nameof(BookDto.Revenue), true);
        AddText(window, nameof(BookDto.Score), true);
        AddText(window, nameof(BookDto.Tags), allowWrap: true);
    }

    protected override async Task UpdateAsync([NotNull] BookDto item)
    {
        var newTags = item.Tags.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (item.Id == -1)
        {
            var book = await _booksRepository.AddBookAsync(item.Title, item.Author, item.ReleaseDate, item.Price,
                item.Count);
            await _tagsRepository.AddBookToTags(book, newTags);
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

            var currentTags = book.Tags.Select(m => m.Name).ToArray();
            var toDelete = currentTags.Except(newTags);
            var toAdd = newTags.Except(currentTags);

            await _tagsRepository.RemoveBookFromTags(book, toDelete);
            await _tagsRepository.AddBookToTags(book, toAdd);
        }

        await RefreshAsync();
    }

    private async Task ShowReviewsAsync(BookDto book)
    {
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<Review, ReviewDto>>();

        await allReviews.ShowBy(
            r =>
            {
                var repo = r.Cast<Review, IReviewsRepository>();
                return repo.GetReviewForBooksAsync(new Book { Id = book.Id });
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
            r =>
            {
                var repo = r.Cast<Order, IOrdersRepository>();
                return repo.GetOrdersForBookAsync(new Book { Id = book.Id });
            },
            null);
    }
}