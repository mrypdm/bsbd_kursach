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
using GuiClient.ViewModels.Abstraction;
using GuiClient.ViewModels.Data;
using GuiClient.ViewModels.Data.Providers.BooksInOrder;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Windows;

public class OrderWindowViewModel : AllEntitiesViewModel<OrderDataViewModel>
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOrderBooksRepository _orderBooksRepository;
    private readonly IBooksRepository _booksRepository;

    public OrderWindowViewModel(ISecurityContext securityContext, IOrdersRepository ordersRepository,
        IOrderBooksRepository orderBooksRepository,
        IBooksRepository booksRepository, IMapper mapper)
        : base(securityContext, mapper)
    {
        _ordersRepository = ordersRepository;
        _orderBooksRepository = orderBooksRepository;
        _booksRepository = booksRepository;

        ShowBooks = new AsyncFuncCommand<OrderDataViewModel>(ShowBooksAsync);
        ShowTotalSum =
            new AsyncFuncCommand<OrderDataViewModel>(ShowTotalSumAsync, item => item is { Id: not -1, TotalSum: null });

        Add = new AsyncActionCommand(AddAsync, () => Provider?.CanCreate == true);
        Update = new AsyncFuncCommand<OrderDataViewModel>(UpdateAsync, item => item?.Id == -1);
        Delete = new AsyncFuncCommand<OrderDataViewModel>(DeleteAsync, item => item?.Id == -1);
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

        window.AddText(nameof(OrderDataViewModel.Id), true);
        window.AddText(nameof(OrderDataViewModel.ClientId), true);
        window.AddText(nameof(OrderDataViewModel.Client), true);
        window.AddText(nameof(OrderDataViewModel.CreatedAt), true);
        window.AddButton(nameof(OrderDataViewModel.TotalSum), nameof(ShowTotalSum), true);
    }

    protected override async Task UpdateAsync([NotNull] OrderDataViewModel item)
    {
        var books = item.Books
            .Select(m => new OrdersToBook { BookId = m.BookId, Count = m.Count })
            .ToList();

        var order = await _ordersRepository.AddOrderAsync(new Client { Id = item.ClientId }, books);
        MessageBox.Show($"Order created with ID={order.Id}");

        await RefreshAsync();
    }

    protected override Task DeleteAsync(OrderDataViewModel item)
    {
        Entities.Remove(item);
        return Task.CompletedTask;
    }

    private async Task ShowTotalSumAsync(OrderDataViewModel item)
    {
        if (item.Id != -1)
        {
            var entities = await _orderBooksRepository.GetBooksForOrderAsync(new Order { Id = item.Id });
            item.Books = Mapper.Map<BookInOrderDataViewModel[]>(entities);
        }
    }

    private async Task ShowBooksAsync(OrderDataViewModel item)
    {
        var entityViewModel = App.ServiceProvider.GetRequiredService<IEntityViewModel<BookInOrderDataViewModel>>();
        var windowViewModel = await entityViewModel.ShowBy(BooksByOrderProvider.Create(item), true);
        item.Books = windowViewModel.Entities;
    }
}