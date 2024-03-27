using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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

public class AllOrdersViewModel : AllEntitiesViewModel<Order, OrderDto>
{
    private readonly Dictionary<OrderDto, IAllEntitiesViewModel<Order, BookInOrderDto>> _viewModels = new();

    private readonly IOrdersRepository _ordersRepository;
    private readonly IBooksRepository _booksRepository;

    public AllOrdersViewModel(ISecurityContext securityContext, IOrdersRepository ordersRepository,
        IBooksRepository booksRepository, IMapper mapper)
        : base(securityContext, ordersRepository, mapper)
    {
        _ordersRepository = ordersRepository;
        _booksRepository = booksRepository;

        ShowBooks = new AsyncFuncCommand<OrderDto>(ShowBooksAsync);
    }

    public ICommand ShowBooks { get; set; }

    public override void EnrichDataGrid(AllEntitiesWindow window)
    {
        base.EnrichDataGrid(window);
        AddButton(window, "Manage books", nameof(ShowBooks));
        AddText(window, nameof(OrderDto.Id), true);
        AddText(window, nameof(OrderDto.ClientId), true);
        AddText(window, nameof(OrderDto.Client), true);
        AddText(window, nameof(OrderDto.CreatedAt), true);
    }

    protected override async Task UpdateAsync([NotNull] OrderDto item)
    {
        if (item.Id != -1)
        {
            throw new NotSupportedException("Cannot update order");
        }

        var viewModel = _viewModels[item];
        _viewModels.Remove(item);

        var books = viewModel.Entities
            .Select(m => new OrdersToBook { BookId = m.BookId, Count = m.Count })
            .ToList();

        var order = await _ordersRepository.AddOrderAsync(new Client { Id = item.ClientId }, books);
        MessageBox.Show($"Order created with ID={order.Id}");
    }

    private async Task ShowBooksAsync(OrderDto item)
    {
        if (!_viewModels.TryGetValue(item, out var viewModel))
        {
            viewModel = App.ServiceProvider.GetRequiredService<IAllEntitiesViewModel<Order, BookInOrderDto>>();

            if (item.Id == -1)
            {
                _viewModels.Add(item, viewModel);

                viewModel.SetDefaultDto(async () =>
                {
                    if (!AskerWindow.TryAskInt("Enter book ID", out var bookId))
                    {
                        return null;
                    }

                    var book = await _booksRepository.GetByIdAsync(bookId)
                        ?? throw new KeyNotFoundException($"Cannot find book with Id={bookId}");

                    return new BookInOrderDto
                    {
                        OrderId = item.Id,
                        BookId = book.Id,
                        Book = book.ToString(),
                        Count = 1
                    };
                });
            }

            viewModel.SetFilter(async r =>
            {
                if (item.Id == -1)
                {
                    return [new Order()];
                }

                var repo = r.Cast<Order, IOrdersRepository>();
                return [await repo.GetByIdAsync(item.Id)];
            });

            await viewModel.RefreshAsync();
        }

        var window = new AllEntitiesWindow(viewModel);
        viewModel.EnrichDataGrid(window);
        window.Show();
    }
}