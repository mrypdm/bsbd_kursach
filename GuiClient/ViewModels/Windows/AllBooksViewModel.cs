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
using GuiClient.ViewModels.Windows.Abstractions;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.Windows;

public class AllBooksViewModel : AllEntitiesWindowViewModel<Book, BookDto>
{
    private readonly BooksRepository _booksRepository;
    private readonly string _param;
    private readonly object _value;

    public AllBooksViewModel(ISecurityContext securityContext, BooksRepository booksRepository, IMapper mapper,
        DtoViewFactory dtoFactory, string param, object value)
        : base(securityContext, booksRepository, mapper, dtoFactory)
    {
        _booksRepository = booksRepository;
        _param = param;
        _value = value;

        ShowReviews = new ActionCommand(Check);
        ShowOrders = new ActionCommand(Check);
    }

    public ICommand ShowReviews { get; }

    public ICommand ShowOrders { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);
        AddButton(window, "Show reviews", nameof(ShowReviews));
        AddButton(window, "Show orders", nameof(ShowOrders));
    }

    private void Check()
    {
        MessageBox.Show("Biba");
    }

    public override async Task RefreshAsync()
    {
        if (_param is null)
        {
            await base.RefreshAsync();
            return;
        }

        var entities = _param switch
        {
            "title" => await _booksRepository.GetBooksByTitleAsync((string)_value),
            "author" => await _booksRepository.GetBooksByAuthorAsync((string)_value),
            "count" => await _booksRepository.GetBooksWithCountLessThanAsync((int)_value),
            "tags" => await _booksRepository.GetBooksByTagsAsync((string[])_value),
            _ => throw new InvalidOperationException("Cannot determine parameter for filter")
        };

        const string prefix = "All of the Books with";
        WindowTitle = _param switch
        {
            "title" => $"{prefix} Title = {_value}",
            "author" => $"{prefix} with Author = {_value}",
            "count" => $"{prefix} with Count < {_value}",
            "tags" => $"{prefix} with Tags: {string.Join(",", (string[])_value)}",
            _ => throw new InvalidOperationException("Cannot determine parameter for filter")
        };

        Entities = Mapper.Map<BookDto[]>(entities);
    }
}