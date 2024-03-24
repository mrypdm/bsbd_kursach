using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dtos;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllBooksViewModel : AllEntitiesViewModel<Book, BookDto>
{
    private readonly BooksRepository _booksRepository;
    private readonly string _filter;
    private readonly TagsRepository _tagsRepository;
    private readonly object _value;

    public AllBooksViewModel(ISecurityContext securityContext, BooksRepository booksRepository,
        TagsRepository tagsRepository, IMapper mapper, string filter, object value)
        : base(securityContext, booksRepository, mapper)
    {
        _booksRepository = booksRepository;
        _tagsRepository = tagsRepository;
        _filter = filter;
        _value = value;

        WindowTitlePostfix = _filter switch
        {
            null => string.Empty,
            "title" => $"with Title = {_value}",
            "author" => $"with Author = {_value}",
            "count" => $"with Count < {_value}",
            "tags" => $"with Tags: {string.Join(",", (string[])_value)}",
            _ => throw new InvalidOperationException("Cannot determine parameter for filter")
        };

        ShowReviews = new ActionCommand(Placeholder);
        ShowOrders = new ActionCommand(Placeholder);
    }

    public ICommand ShowReviews { get; }

    public ICommand ShowOrders { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);
        AddButton(window, "Show reviews", nameof(ShowReviews));
        AddButton(window, "Show orders", nameof(ShowOrders));
        AddText(window, nameof(BookDto.Id), true);
        AddText(window, nameof(BookDto.Title));
        AddText(window, nameof(BookDto.Author));
        AddText(window, nameof(BookDto.ReleaseDate));
        AddText(window, nameof(BookDto.Count));
        AddText(window, nameof(BookDto.Price));
        AddText(window, nameof(BookDto.Tags), allowWrap: true);
    }

    // TODO
    private void Placeholder()
    {
        MessageBox.Show("Not implemented");
    }

    public override async Task RefreshAsync()
    {
        if (_filter is null)
        {
            await base.RefreshAsync();
            return;
        }

        var entities = _filter switch
        {
            "title" => await _booksRepository.GetBooksByTitleAsync((string)_value),
            "author" => await _booksRepository.GetBooksByAuthorAsync((string)_value),
            "count" => await _booksRepository.GetBooksWithCountLessThanAsync((int)_value),
            "tags" => await _booksRepository.GetBooksByTagsAsync((string[])_value),
            _ => throw new InvalidOperationException("Cannot determine parameter for filter")
        };

        Entities = Mapper.Map<BookDto[]>(entities);
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
            var book = await _booksRepository.GetById(item.Id);
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
}