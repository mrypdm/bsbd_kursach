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

public class AllOrdersViewModel : AllEntitiesViewModel<OrderDto>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOrderBooksRepository _orderBooksRepository;
    private readonly IBooksRepository _booksRepository;

    public AllOrdersViewModel(ISecurityContext securityContext, IOrdersRepository ordersRepository,
        IOrderBooksRepository orderBooksRepository,
        IBooksRepository booksRepository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _ordersRepository = ordersRepository;
        _orderBooksRepository = orderBooksRepository;
        _booksRepository = booksRepository;

        ShowBooks = new AsyncFuncCommand<OrderDto>(ShowBooksAsync);
        ShowTotalSum =
            new AsyncFuncCommand<OrderDto>(ShowTotalSumAsync, item => item is { Id: not -1, TotalSum: null });

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<OrderDto>(UpdateAsync, item => item?.Id == -1);
        Delete = new AsyncFuncCommand<OrderDto>(DeleteAsync, item => item?.Id == -1);
    }

    public ICommand ShowBooks { get; }

    public ICommand ShowTotalSum { get; }

    public override void SetupDataGrid(AllEntitiesWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);
        window.Clear();

        window.AddButton("Delete", nameof(Delete));
        window.AddButton("Update", nameof(Update));
        window.AddButton("Show books", nameof(ShowBooks));

        window.AddText(nameof(OrderDto.Id), true);
        window.AddText(nameof(OrderDto.ClientId), true);
        window.AddText(nameof(OrderDto.Client), true);
        window.AddText(nameof(OrderDto.CreatedAt), true);
        window.AddButton(nameof(OrderDto.TotalSum), nameof(ShowTotalSum), true);
    }

    protected override async Task UpdateAsync([NotNull] OrderDto item)
    {
        var books = item.Books
            .Select(m => new OrdersToBook { BookId = m.BookId, Count = m.Count })
            .ToList();

        var order = await _ordersRepository.AddOrderAsync(new Client { Id = item.ClientId }, books);
        MessageBox.Show($"Order created with ID={order.Id}");

        await RefreshAsync();
    }

    protected override Task DeleteAsync(OrderDto item)
    {
        Entities.Remove(item);
        return Task.CompletedTask;
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
        var entityViewModel = App.ServiceProvider.GetRequiredService<IEntityViewModel<BookInOrderDto>>();
        var windowViewModel = await entityViewModel.ShowBy(BooksByOrderProvider.Create(item), true);
        item.Books = windowViewModel.Entities;
    }
}