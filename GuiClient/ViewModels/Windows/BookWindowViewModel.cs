using System;
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
using GuiClient.DtoProviders.Orders;
using GuiClient.DtoProviders.Reviews;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class BookWindowViewModel : AllEntitiesViewModel<BookDto>
{
    private readonly IBooksRepository _booksRepository;
    private readonly ITagsRepository _tagsRepository;

    public BookWindowViewModel(ISecurityContext securityContext, IBooksRepository booksRepository,
        ITagsRepository tagsRepository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _booksRepository = booksRepository;
        _tagsRepository = tagsRepository;

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

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<BookDto>(UpdateAsync);
        Delete = new AsyncFuncCommand<BookDto>(DeleteAsync, item => item?.Id == -1 || IsAdmin);
    }

    public ICommand ShowReviews { get; }

    public ICommand ShowOrders { get; }

    public ICommand ShowRevenue { get; }

    public ICommand ShowTags { get; }

    public ICommand ShowScore { get; }

    public ICommand ShowSales { get; }

    public override void SetupDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        window.Clear();

        window.AddButton("Delete", nameof(Delete));
        window.AddButton("Update", nameof(Update));
        window.AddButton("Show reviews", nameof(ShowReviews));
        window.AddButton("Show orders", nameof(ShowOrders));

        window.AddText(nameof(BookDto.Id), true);
        window.AddText(nameof(BookDto.Title));
        window.AddText(nameof(BookDto.Author));
        window.AddText(nameof(BookDto.ReleaseDate));
        window.AddText(nameof(BookDto.Count));
        window.AddText(nameof(BookDto.Price));
        window.AddButton(nameof(BookDto.Sales), nameof(ShowSales), true);
        window.AddButton(nameof(BookDto.Revenue), nameof(ShowRevenue), true);
        window.AddButton(nameof(BookDto.Score), nameof(ShowScore), true);
        window.AddButton(nameof(BookDto.Tags), nameof(ShowTags), true);
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

    protected override async Task DeleteAsync([NotNull] BookDto item)
    {
        if (item.Id == -1)
        {
            Entities.Remove(item);
            return;
        }

        await _booksRepository.RemoveAsync(new Book { Id = item.Id });
        await RefreshAsync();
    }

    private async Task ShowReviewsAsync(BookDto book)
    {
        var allReviews = App.ServiceProvider.GetRequiredService<IEntityViewModel<ReviewDto>>();
        await allReviews.ShowBy(ReviewsByBookProvider.Create(book));
    }

    private async Task ShowOrdersAsync(BookDto book)
    {
        var allOrders = App.ServiceProvider.GetRequiredService<IEntityViewModel<OrderDto>>();
        await allOrders.ShowBy(OrdersByBookProvider.Create(book));
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