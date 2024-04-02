using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.DtoProviders.BooksInOrder;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class AllOrdersViewModel : AllEntitiesViewModel<Order, OrderDto>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOrderBooksRepository _orderBooksRepository;
    private readonly IBooksRepository _booksRepository;

    public AllOrdersViewModel(ISecurityContext securityContext, IOrdersRepository ordersRepository,
        IOrderBooksRepository orderBooksRepository,
        IBooksRepository booksRepository, IMapper mapper)
        : base(securityContext, ordersRepository, mapper)
    {
        _ordersRepository = ordersRepository;
        _orderBooksRepository = orderBooksRepository;
        _booksRepository = booksRepository;

        Update = new AsyncFuncCommand<OrderDto>(UpdateAsync, item => item?.Id == -1);

        ShowBooks = new AsyncFuncCommand<OrderDto>(ShowBooksAsync);
        ShowTotalSum =
            new AsyncFuncCommand<OrderDto>(ShowTotalSumAsync, item => item is { Id: not -1, TotalSum: null });
    }

    public ICommand ShowBooks { get; }

    public ICommand ShowTotalSum { get; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        AddButton(window, "Update", nameof(Update));
        AddButton(window, "Manage books", nameof(ShowBooks));
        AddText(window, nameof(OrderDto.Id), true);
        AddText(window, nameof(OrderDto.ClientId), true);
        AddText(window, nameof(OrderDto.Client), true);
        AddText(window, nameof(OrderDto.CreatedAt), true);
        AddButton(window, nameof(OrderDto.TotalSum), nameof(ShowTotalSum), true);
    }

    protected override async Task UpdateAsync([NotNull] OrderDto item)
    {
        if (item.Id != -1)
        {
            throw new NotSupportedException("Cannot update order");
        }

        var books = item.Books
            .Select(m => new OrdersToBook { BookId = m.BookId, Count = m.Count })
            .ToList();

        var order = await _ordersRepository.AddOrderAsync(new Client { Id = item.ClientId }, books);
        MessageBox.Show($"Order created with ID={order.Id}");

        await RefreshAsync();
    }

    private async Task ShowTotalSumAsync(OrderDto item)
    {
        if (item.Id != -1)
        {
            var entities = await _orderBooksRepository.GetBooksForOrderAsync(new Order { Id = item.Id });
            item.Books = Mapper.Map<BookInOrderDto[]>(entities);
        }
    }

    private async Task ShowBooksAsync(OrderDto item)
    {
        // TODO: as IEntityViewModel
        var viewModel = App.ServiceProvider.GetRequiredService<IAllEntitiesViewModel<OrdersToBook, BookInOrderDto>>();

        viewModel.SetProvider(BooksByOrderProvider.Create(item));

        await viewModel.RefreshAsync();

        var window = new AllEntitiesWindow(viewModel);
        viewModel.EnrichDataGrid(window);
        window.ShowDialog();

        item.Books = viewModel.Entities;
    }
}