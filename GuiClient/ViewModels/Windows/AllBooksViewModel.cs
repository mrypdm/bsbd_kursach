using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dtos;
using GuiClient.Factories;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllBooksViewModel : AllEntitiesViewModel<Book, BookDto>
{
    private readonly BooksRepository _booksRepository;
    private readonly string _filter;
    private readonly object _value;

    public AllBooksViewModel(ISecurityContext securityContext, BooksRepository booksRepository, IMapper mapper,
        DtoViewFactory dtoFactory, string filter, object value)
        : base(securityContext, booksRepository, mapper, dtoFactory)
    {
        _booksRepository = booksRepository;
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
}